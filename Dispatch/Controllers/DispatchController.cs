using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Dispatch.Controllers;

[ApiController]
[Route("[controller]")]
public class DispatchController : ControllerBase
{
    // private readonly ILogger<DispatchController> _logger;
    private readonly GeoLocation _homeLocation;
    private readonly Queue<AssignDeliveryRequest> _unfilledOrders;
    private readonly IFleetRepository _fleet;
    private readonly IOrdersRepository _orders;
    private readonly DispatchToSimDroneGateway _dispatchToSimDroneGateway;
        
    public DispatchController(
        IFleetRepository droneRepo,
        // DroneGateway droneGateway,
        IOrdersRepository orderRepo
        // GeoLocation homeLocation,
        // Queue<AssignDeliveryRequest> unfilledOrders
        )
    {
        _fleet = droneRepo;
        _orders = orderRepo;
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(droneRepo/*, orderRepo*/);
        _unfilledOrders = new Queue<AssignDeliveryRequest>();
        _homeLocation = new GeoLocation
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00858710385576m
        };
    }

        
    [HttpPost("Ping")]
    public async Task<string> Ping(PingDto name)
    {
        var greeting = $"Hello, {name} from Dispatch";
        // Response.AppendHeader("Access-Control-Allow-Origin", "*");
        // .WriteAsJsonAsync() 
        Console.WriteLine(greeting);
        return greeting;
    }
    
    [HttpPost("AddDrone")]
    public async Task<AddDroneResponse> AddDrone(AddDroneRequest addDroneRequest)
    {
        addDroneRequest.DispatchUrl = "http://localhost:83";
        addDroneRequest.HomeLocation = new GeoLocation { Latitude = 39.74386695629378m, Longitude = -105.00610500179027m };
        Console.WriteLine($"DispatchController.AddDrone({addDroneRequest})");

        if((await _fleet.GetAllAsync())
           .Any(x => x.BadgeNumber == addDroneRequest.BadgeNumber 
                  || x.DroneUrl == addDroneRequest.DroneUrl
                  || x.DroneId == addDroneRequest.DroneUrl))
        {
            Console.WriteLine("Either the DroneUrl or DroneId you are trying to use is taken by another drone.");
            return new AddDroneResponse
            {
                BadgeNumber = addDroneRequest.BadgeNumber,
                Success = false
            };
        }
        var initDroneRequest = new InitDroneRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneUrl = addDroneRequest.DroneUrl
        };

        var initDroneResponse = _dispatchToSimDroneGateway
           .InitDrone( initDroneRequest ).Result;
        Console.WriteLine($"Response from _dispatchToSimDroneGateway.InitDrone({initDroneRequest})\n\t->{{DroneId:{initDroneResponse.DroneId},Okay:{initDroneResponse.Okay}}}");
        if (!initDroneResponse.Okay)
        {
            return new AddDroneResponse { BadgeNumber = addDroneRequest.BadgeNumber, Success = false };
        }

        var assignFleetRequest = new AssignFleetRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneIp = addDroneRequest.DroneUrl,
            DispatchIp = addDroneRequest.DispatchUrl,
            BadgeNumber = addDroneRequest.BadgeNumber,
            HomeLocation = addDroneRequest.HomeLocation
        };

        Console.WriteLine($"Proceeding with _dispatchToSimDroneGateway.AssignFleet({assignFleetRequest.ToJson()}");
        var assignFleetResponse = _dispatchToSimDroneGateway.AssignFleet(assignFleetRequest).Result;
        var responseString = assignFleetResponse != null ? assignFleetResponse.IsInitializedAndAssigned.ToString() : "null";
        Console.WriteLine($"\t_dispatchToSimDroneGateway.AssignFleet - response -> {responseString}");

        if(assignFleetResponse is {IsInitializedAndAssigned: false})
        {
            Console.WriteLine($"FAILURE! new drone {addDroneRequest.BadgeNumber} was not initiated.");
            return new AddDroneResponse
            {
                BadgeNumber = addDroneRequest.BadgeNumber,
                Success = false
            };
        }
        Console.WriteLine($"success! Saving new drone {addDroneRequest.BadgeNumber} to repository.");

        var droneRecord = new DroneRecord
        {
            OrderId = null,
            DroneId = addDroneRequest.DroneId,
            DroneUrl = addDroneRequest.DroneUrl,
            BadgeNumber = addDroneRequest.BadgeNumber,
            Destination = addDroneRequest.HomeLocation,
            CurrentLocation = addDroneRequest.HomeLocation,
            HomeLocation = addDroneRequest.HomeLocation,
            DispatchUrl = addDroneRequest.DispatchUrl,
            State = assignFleetResponse.FirstState
        };

        await _fleet.CreateAsync(
            droneRecord);

        return new AddDroneResponse
        {
            BadgeNumber = addDroneRequest.BadgeNumber,
            Success = true
        };
    }
    
    [HttpPost("CompleteOrder")]
    public async Task<bool> CompleteOrder(CompleteOrderRequest completeOrderRequest)
    {
        Console.WriteLine($"DispatchController.CompleteOrder -> {completeOrderRequest}");

        var orderUpdateSuccess = await _orders.UpdateOrderCompletionTime(completeOrderRequest.OrderId, DateTime.Now);

        var unfilledOrders = (await _orders.GetAllAsync()).Where(order => order.HasBeenDelivered == false);
        if (unfilledOrders.Any())
        {
            Console.WriteLine($"Incomplete orders exist, attempting to assign a newly returned drone");
            await _dispatchToSimDroneGateway.AssignDelivery(new AssignDeliveryRequest
            {
                DroneId = completeOrderRequest.DroneId,
                OrderId = unfilledOrders.First().OrderId,
                OrderLocation = unfilledOrders.First().DeliveryLocation
            });
        }
        return orderUpdateSuccess;
    }

    [HttpPost("EnqueueOrder")]
    public async Task<AssignDeliveryResponse> EnqueueOrder(EnqueueOrderRequest enqueueOrderRequest)
    {
        Console.WriteLine($"DispatchController.EqueueOrder -> {enqueueOrderRequest}");
        var availableDrones = (await _fleet.GetAllAsync()).Where(drone => drone.State == DroneState.Ready);
        if (availableDrones.Any())
        {
            var droneId = availableDrones.First().DroneId;
            return await _dispatchToSimDroneGateway.AssignDelivery(new AssignDeliveryRequest
            {
                DroneId = droneId,
                OrderId = enqueueOrderRequest.OrderId,
                OrderLocation = enqueueOrderRequest.OrderLocation
            });
        }
        else return new AssignDeliveryResponse
        {
            OrderId = enqueueOrderRequest.OrderId,
            DroneId = null,
            Success = false
        };
    }

    [HttpPost("PostInitialStatus")]
    public async Task<UpdateDroneStatusResponse> PostInitialStatus(UpdateDroneStatusRequest initialStatusRequest)
    {
        Console.WriteLine($"DispatchController.PostInitialStatus -> {initialStatusRequest}");
        return await UpdateDroneStatus(initialStatusRequest);
    }
        
    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse> UpdateDroneStatus(UpdateDroneStatusRequest droneStatusRequest)
    {
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");
        var droneRecord = new DroneRecord
        {
            DroneId = droneStatusRequest.DroneId,
            CurrentLocation = droneStatusRequest.Location,
            State = droneStatusRequest.State
        };

        var response = new UpdateDroneStatusResponse
        {
            DroneId = droneStatusRequest.DroneId,
            IsCompletedSuccessfully = false
        };

        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");
        if (droneStatusRequest.State != DroneState.Ready ||  _unfilledOrders.Count <= 0)
        {
            Console.WriteLine($"\n\n\nDrone {droneStatusRequest.DroneId} is still delivering an order. Updating the status tho....");
            response.IsCompletedSuccessfully = await _fleet.UpdateStatusAndLocationAsync(droneRecord);
            Console.WriteLine($"The status of {droneStatusRequest.DroneId}'s db update is {response.IsCompletedSuccessfully}\n\n\n");
        }
        else
        {
            var orderDto = _unfilledOrders.Dequeue();
            Console.WriteLine($"Drone i{droneStatusRequest.DroneId} is ready for the next order, and we have more. Resending to order {orderDto.OrderId}\n\n\n");
            orderDto.DroneId = droneStatusRequest.DroneId;
            await _dispatchToSimDroneGateway.AssignDelivery(orderDto);
            response.IsCompletedSuccessfully = await _fleet.UpdateStatusAndLocationAsync(droneRecord);
        }

        return response;
    }

        
    [HttpPost("{id:length(24)}")]
    public async Task<ActionResult<DroneRecord>> GetDroneById(string requestedDroneId)
    {
        Console.WriteLine($"DispatchController.Get -> {requestedDroneId}");
        var droneRecord = await _fleet.GetByIdAsync(requestedDroneId);

        if (droneRecord is null)
        {
            return NotFound();
        }
        return droneRecord;
    }
}