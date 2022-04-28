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
            OrderId = "",
            DroneId = addDroneRequest.DroneId,
            DroneUrl = addDroneRequest.DroneUrl,
            BadgeNumber = addDroneRequest.BadgeNumber,
            Destination = addDroneRequest.HomeLocation,
            CurrentLocation = addDroneRequest.HomeLocation,
            HomeLocation = addDroneRequest.HomeLocation,
            DispatchUrl = addDroneRequest.DispatchUrl,
            State = DroneState.Charging
        };

        await _fleet.CreateAsync(
            droneRecord);

        Console.WriteLine($"\n\n\n\nabout to YEET this drone record:\n{droneRecord.ToJson()}");
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
        var order = new Order
        {
            DroneId = completeOrderRequest.OrderId,
            TimeDelivered = DateTime.Now
        };
        var result = await _orders.UpdateAsync(order);
        return result.IsAcknowledged && result.ModifiedCount == 0;
    }

    [HttpPost("EnqueueOrder")]
    public async Task<AssignDeliveryResponse?> EnqueueOrder(EnqueueOrderRequest request)
    {
        Console.WriteLine($"DispatchController.EnqueueOrder -> {request}");
        var availableDrones = await GetAvailableDrones();
        var drones = availableDrones as DroneRecord[] ?? availableDrones.ToArray();
        if (!drones.Any())
        {
            Console.WriteLine($"\n\nNo available drones at this time.");
            return new AssignDeliveryResponse {OrderId = request.OrderId, Success = false};
        }
        Console.WriteLine($"\n\nAvailable drones are:\n{string.Join("\n", drones.Select(x => x.ToString()))}");
        return await InitiateDelivery(new Order{Id = request.OrderId, DeliveryLocation = request.OrderLocation}, drones.First());
    }

    private async Task<AssignDeliveryResponse?> InitiateDelivery(Order order, DroneRecord drone)
    {
        drone.OrderId = order.Id;
        drone.State = DroneState.Preparing;
        await _fleet.UpdateAsync(drone);
        var assignDeliveryResponse = await _dispatchToSimDroneGateway.AssignDelivery(new AssignDeliveryRequest
        {
            DroneId = drone.DroneId,
            OrderId = order.Id,
            OrderLocation = order.DeliveryLocation
        });
        return assignDeliveryResponse;
    }

    public async Task<IEnumerable<Task<AssignDeliveryResponse?>>> DequeueOrders()
    {
        Console.WriteLine("DequeueOrders...");
        var orders = await GetUnfulfilledOrders();
        return from drone in await GetAvailableDrones()
        from order in orders
        select InitiateDelivery(order, drone);
    }
    
    private async Task<IEnumerable<DroneRecord>> GetAvailableDrones()
    {
        Console.WriteLine("DequeueOrders...");
        var drones = from d in await _fleet.GetAllAsync()
            where d.State == DroneState.Ready && d.OrderId.Equals("")
            select d;
        return drones;
    }

    private async Task<IEnumerable<Order>> GetUnfulfilledOrders()
    {
        var orders = from o in await _orders.GetAllAsync()
            where !o.HasBeenDelivered && o.DroneId.Equals(string.Empty)
            select o;
        Console.WriteLine(string.Join(",", orders));
        return orders;
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
            State = droneStatusRequest.State,
        };
         var updateResult = await _fleet.UpdateAsync(droneRecord);
         var response = new UpdateDroneStatusResponse
        {
            DroneId = droneStatusRequest.DroneId,
            IsCompletedSuccessfully = updateResult.IsAcknowledged && updateResult.ModifiedCount == 1 
        };
         Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");
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