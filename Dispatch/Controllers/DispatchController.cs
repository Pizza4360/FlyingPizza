using System.Diagnostics;
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
    private readonly DispatchToSimDroneGateway _dispatchToSimDroneGateway;

    private readonly IFleetRepository _fleet;

    // private readonly ILogger<DispatchController> _logger;
    private readonly GeoLocation _homeLocation;
    private readonly IOrdersRepository _orders;


    private readonly Stopwatch _stopwatch;
    private bool isInitiatingDrone;

    // private async void DequeueCallback(object _) => await TryDequeueOrders();
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
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(droneRepo /*, orderRepo*/);
        _homeLocation = new GeoLocation
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00858710385576m
        };
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    [HttpPost("Revive")]
    public async Task<bool> Revive(DroneRecord record)
    {
        var possibleDrones = (await _fleet.GetAllAsync()).Where(x => x.DroneId.Equals(record.DroneId)).ToList();
        if (!possibleDrones.Any() || possibleDrones.Count != 1 
                                  || ! (await _dispatchToSimDroneGateway.HealthCheck(possibleDrones.First().DroneUrl)))
        {
            return false;
        }
        await _fleet.UpdateAsync(possibleDrones.First().Update());
        return true;
    }

    [HttpPost("AssgnmentCheck")]
    public async Task<PingDto> AssgnmentCheck(PingDto p)
    {
        if (isInitiatingDrone) return p;
        Console.WriteLine("Trying to dequeue some orders...");
        var orders = (from o in await GetUnfulfilledOrders() where string.IsNullOrEmpty(o.DroneId) select o).ToList();
        // Console.WriteLine(string.Join("\n", orders.ToJson()));
        var availableDrones =
            (from d in await GetAvailableDrones() where string.IsNullOrEmpty(d.OrderId) select d).ToList();
        Console.WriteLine(
            $"\n\n\n\navailable drones: {availableDrones.Count}\nunfulfilled orders: {orders.Count}\n");
        var assignments = from drone in availableDrones
            from order in orders
            where order.State == OrderState.Waiting
            select new AssignDeliveryRequest
            {
                OrderId = order.OrderId,
                OrderLocation = order.DeliveryLocation,
                DroneId = drone.DroneId
            };
        foreach (var delivery in assignments)
        {
            Console.WriteLine($"Matched some drones with deliveries...\n{assignments.ToJson()}");
            var responseString = await InitiateDelivery(delivery);
            if (responseString.Success)
                Console.WriteLine(responseString.ToJson());
        }

        return p;
    }

    [HttpPost("AddDrone")]
    public async Task<AddDroneResponse> AddDrone(AddDroneRequest addDroneRequest)
    {
        isInitiatingDrone = true;
        addDroneRequest.DispatchUrl = "http://localhost:83";
        addDroneRequest.HomeLocation = new GeoLocation
            {Latitude = 39.74386695629378m, Longitude = -105.00610500179027m};
        Console.WriteLine($"\n\n\n\nDispatchController.AddDrone({addDroneRequest})\n\n\n\n");

        if ((await _fleet.GetAllAsync())
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
            .InitDrone(initDroneRequest).Result;
        Console.WriteLine(
            $"\n\n\n\nResponse from _dispatchToSimDroneGateway.InitDrone({initDroneRequest})\n\t->{{DroneId:{initDroneResponse.DroneId},Okay:{initDroneResponse.Okay}}}\n\n\n\n");
        if (!initDroneResponse.Okay)
            return new AddDroneResponse {BadgeNumber = addDroneRequest.BadgeNumber, Success = false};

        var assignFleetRequest = new AssignFleetRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneIp = addDroneRequest.DroneUrl,
            DispatchUrl = addDroneRequest.DispatchUrl,
            BadgeNumber = addDroneRequest.BadgeNumber,
            HomeLocation = addDroneRequest.HomeLocation
        };

        Console.WriteLine($"\n\n\n\nProceeding with _dispatchToSimDroneGateway.AssignFleet({assignFleetRequest.ToJson()}\n\n\n\n");
        var assignFleetResponse = _dispatchToSimDroneGateway.AssignFleet(assignFleetRequest).Result;
        var responseString = assignFleetResponse != null
            ? assignFleetResponse.IsInitializedAndAssigned.ToString()
            : "null";
        Console.WriteLine($"\n\n\n\n_dispatchToSimDroneGateway.AssignFleet - response -> {responseString}\n\n\n\n");

        if (assignFleetResponse is {IsInitializedAndAssigned: false})
        {
            Console.WriteLine($"FAILURE! new drone {addDroneRequest.BadgeNumber} was not initiated.");
            return new AddDroneResponse
            {
                BadgeNumber = addDroneRequest.BadgeNumber,
                Success = false
            };
        }

        Console.WriteLine($"\n\n\n\nsuccess! Saving new drone {addDroneRequest.BadgeNumber} to repository.\n\n\n\n");

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
            State = DroneState.Ready
        };

        await _fleet.CreateAsync(
            droneRecord);

        Console.WriteLine($"\n\n\n\nabout to YEET this drone record:\n{droneRecord.ToJson()}");
        isInitiatingDrone = false;
        return new AddDroneResponse
        {
            BadgeNumber = addDroneRequest.BadgeNumber,
            Success = true
        };
    }

    [HttpPost("CompleteOrder")]
    public async Task<CompleteOrderResponse> CompleteOrder(CompleteOrderRequest request)
    {
        var order = new Order
        {
            OrderId = request.OrderId,
            State = OrderState.Delivered,
            TimeDelivered = request.Time,
            DroneId = request.DroneId
        };
        Console.WriteLine(
            $"order {order.OrderId} has been delivered? {order.HasBeenDelivered} @ time {order.TimeDelivered}");
        var result = await _orders.UpdateAsync(order.Update());
        return new CompleteOrderResponse
        {
            IsAcknowledged = result.IsAcknowledged
        };
    }

    [HttpPost("AssignDelivery")]
    private async Task<AssignDeliveryResponse> InitiateDelivery(AssignDeliveryRequest request)
    {
        if (request == null)
        {
            Console.WriteLine("request in Dispatch.InitiateDelivery is null!! ");
            return new AssignDeliveryResponse {DroneId = request.DroneId, Success = false};
        }

        Console.WriteLine("!!!!!!" + request.ToJson());
        Console.WriteLine($"In DispatchController.AssignDeliveryResponse, order :{request.ToJson()}");

        var order = await _orders.GetByIdAsync(request.OrderId);
        var drone = await _fleet.GetByIdAsync(request.DroneId);

        var s1 = $"drone.OrderId {drone.OrderId} -> ";
        var s2 = $"order.DroneId {order.DroneId} -> ";
        order.DroneId = drone.DroneId;
        drone.OrderId = order.OrderId;
        s1 += $"{drone.OrderId}";
        s2 += $"{order.DroneId}";

        Console.WriteLine($"{s1}\n{s2}");
        order.State = OrderState.Assigned;
        drone.State = DroneState.Assigned;

        await _fleet.UpdateAsync(drone.Update());
        await _orders.UpdateAsync(order.Update());

        return await _dispatchToSimDroneGateway.AssignDelivery(request);
    }


    [NonAction]
    private async Task<IEnumerable<Order>> GetUnfulfilledOrders()
    {
        var orders = await _orders.GetAllAsync();
        Console.WriteLine($"All the orders: {orders.Count}");
        var unassignedOrders = orders.Where(o =>
            !o.HasBeenDelivered && o is {State: OrderState.Waiting} && string.IsNullOrEmpty(o.DroneId));
        return unassignedOrders;
    }

    [NonAction]
    private async Task<IEnumerable<DroneRecord>> GetAvailableDrones()
    {
        if (isInitiatingDrone)
        {
            Console.WriteLine("Thread lock, cannot read drones at this time.");
            return new List<DroneRecord>();
        }
        try
        {
            Console.WriteLine("DequeueOrders...");
            var drones = from d in await _fleet.GetAllAsync()
                where d != null
                      && d.State == DroneState.Ready
                      && string.IsNullOrEmpty(d.OrderId)
                select d;
            return drones;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<DroneRecord>();
        }
    }

    [HttpPost("PostInitialStatus")]
    public async Task<UpdateDroneStatusResponse> PostInitialStatus(DroneUpdate initialStatusRequest)
    {
        Console.WriteLine($"DispatchController.PostInitialStatus -> {initialStatusRequest}");

        return await UpdateDroneStatus(initialStatusRequest);
    }

    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse> UpdateDroneStatus(DroneUpdate droneStatusRequest)
    {
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");
        var updateResult = await _fleet.UpdateAsync(droneStatusRequest);
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

        if (droneRecord is null) return NotFound();

        return droneRecord;
    }
}