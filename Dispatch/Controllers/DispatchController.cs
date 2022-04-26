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
    private readonly ICompositeRepository _repository;
    private readonly DispatchToSimDroneGateway _dispatchToSimDroneGateway;
        
    public DispatchController(ICompositeRepository repository)
    {
        _repository = repository;
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(repository);
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
        Console.WriteLine($"DispatchController.AddDrone({addDroneRequest})");

        if(!await CanAddDrone(addDroneRequest.DroneId))
        {
            Console.WriteLine("Either the DroneUrl or DroneUrl you are trying to use is taken by another drone.");
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
        Console.WriteLine($"Response from _dispatchToSimDroneGateway.InitDrone({initDroneRequest})\n\t->{{DroneUrl:{initDroneResponse.DroneId},Okay:{initDroneResponse.Okay}}}");
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
            Orders = null,
            DroneId = addDroneRequest.DroneId,
            DroneUrl = addDroneRequest.DroneUrl,
            BadgeNumber = addDroneRequest.BadgeNumber,
            Destination = addDroneRequest.HomeLocation,
            CurrentLocation = addDroneRequest.HomeLocation,
            HomeLocation = addDroneRequest.HomeLocation,
            DispatchUrl = addDroneRequest.DispatchUrl,
            State = assignFleetResponse.FirstState
        };

        Console.WriteLine($"\n\n\n\nabout to YEET this drone record:\n{droneRecord.ToJson()}");
        await _repository.CreateDroneAsync(droneRecord);
        return new AddDroneResponse
        {
            BadgeNumber = addDroneRequest.BadgeNumber,
            Success = true
        };
    }


    private async Task<bool> CanAddDrone(string droneId)
    {
        return (await _repository.GetDrones()).Any(x => x.DroneId == droneId);
    }


    [HttpPost("CompleteOrder")]
    public async Task<bool> CompleteOrder(CompleteOrderRequest completeOrderRequest)
    {
        Console.WriteLine($"DispatchController.CompleteOrder -> {completeOrderRequest}");
        return await _repository.UpdateOrderAsync(new Order{OrderId = completeOrderRequest.OrderId, TimeDelivered = DateTime.Now});
    }

    [HttpPost("EnqueueOrder")]
    public async Task<EnqueueOrderResponse?> EnqueueOrder(EnqueueOrderRequest enqueueOrderRequest)
    {
        Console.WriteLine($"DispatchController.EqueueOrder -> {enqueueOrderRequest}");
        await _repository.CreateOrderAsync(enqueueOrderRequest.Order);

        return new EnqueueOrderResponse
        {
            IsEnqueued = true
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
        await _repository.UpdateDroneAsync(droneRecord);

        var response = new UpdateDroneStatusResponse
        {
            DroneId = droneStatusRequest.DroneId,
            IsCompletedSuccessfully = false
        };

        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");
        if (droneStatusRequest.State != DroneState.Ready ||
            _unfilledOrders.Count <= 0)
        {
            Console.WriteLine($"\n\n\nDrone {droneStatusRequest.DroneId} is still delivering an order. Updating the status tho....");
            response.IsCompletedSuccessfully = await _repository.UpdateDroneAsync(droneRecord);
            Console.WriteLine($"The status of {droneStatusRequest.DroneId}'s db update is {response.IsCompletedSuccessfully}\n\n\n");
        }
        else
        {
            var orderDto = _unfilledOrders.Dequeue();
            Console.WriteLine($"Drone i{droneStatusRequest.DroneId} is ready for the next order, and we have more. Resending to order {orderDto.Order}\n\n\n");
            orderDto.DroneUrl = droneStatusRequest.DroneId;
            await _dispatchToSimDroneGateway.AssignDelivery(orderDto);
            response.IsCompletedSuccessfully = await _repository.UpdateDroneAsync(droneRecord);
        }
        return response;
    }

        
    [HttpPost("{id:length(24)}")]
    public async Task<ActionResult<DroneRecord>> GetDroneById(string requestedDroneId)
    {
        Console.WriteLine($"DispatchController.Get -> {requestedDroneId}");
        var droneRecord = await _repository.GetDroneByIdAsync(requestedDroneId);

        if (droneRecord is null)
        {
            return NotFound();
        }
        return droneRecord;
    }
}