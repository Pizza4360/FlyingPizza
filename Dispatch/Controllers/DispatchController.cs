using System.Diagnostics;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Dispatch.Controllers;

public class Assignment
{
    public string DroneId { get; set; }
    public string OrderId { get; set; }
    public bool Available => string.IsNullOrEmpty(OrderId);
    public override bool Equals(object o) => o is Assignment a && a.DroneId.Equals(OrderId);
}


public class CircularQueueSet<T>
{
    private readonly Queue<T> q;
    private readonly HashSet<T> h;
    public CircularQueueSet()
    {
        q = new Queue<T>();
        h = new HashSet<T>();
    }

    public T Peek() => q.Peek();
    public void Enqueue(T thing)
    {
        if(h.Contains(thing)) return;
        q.Enqueue(thing);
        h.Add(thing);
    }

    public bool Remove(T item)
    {
        if (!h.Remove(item)) return false;
        while (!q.Peek()!.Equals(item))
        {
            Rotate();
        }
        q.Dequeue();
        return true;
    }

    public T Rotate()
    {
        var temp = q.Dequeue();
        q.Enqueue(temp);
        return temp;
    }
}
[ApiController]
[Route("[controller]")]
public class DispatchController : ControllerBase
{
    private readonly DispatchToSimDroneGateway _dispatchToSimDroneGateway;
    // private readonly List<KeyValuePair<string, Tuple<DroneRecord, Order?, Mutex>>> queue;
    private readonly int queueIndex = 0;
    private readonly IFleetRepository _fleet;
    private readonly IOrdersRepository _orders;
    private readonly GeoLocation _homeLocation;
    private readonly string _dispatchUrl;
    private bool _isInitiatingDrone;
    private bool _assignmentLock;
    private Stopwatch doNothingWatch;
    private bool doNothingLock;
    private CircularQueueSet<Assignment> _assignments;

    public DispatchController( IFleetRepository droneRepo, IOrdersRepository orderRepo )
    {
        _dispatchUrl = Environment.GetEnvironmentVariable("DISPATCH_URL") ?? throw new InvalidOperationException();
        _fleet = droneRepo;
        _orders = orderRepo;
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(droneRepo);
        _homeLocation = new GeoLocation
        {
            Latitude = decimal.Parse(Environment.GetEnvironmentVariable("HOME_LATITUDE") ??
                                     throw new ArgumentException(
                                         "HOME_LATITUDE must be a valid floating point number. Please check environment variables.")),
            Longitude = decimal.Parse(Environment.GetEnvironmentVariable("HOME_LONGITUDE") ??
                                      throw new ArgumentException(
                                          "HOME_LONGITUDE must be a valid floating point number. Please check environment variables."))
        };
        _assignments = new CircularQueueSet<Assignment>();
        doNothingWatch = new Stopwatch();
        foreach (var drone in _fleet.GetAllAsync().Result)
        {
            _assignments.Enqueue(new Assignment{DroneId = drone.DroneId, OrderId = drone.OrderId});
        }
        Console.WriteLine(
            $"Dispatch online at '{_dispatchUrl}', Home location: {_homeLocation.Latitude},{_homeLocation.Longitude}");
    }

    [HttpPost("Recover")]
    public async Task<bool> Recover(DroneRecord record)
    {
        var unsuccessful = true;
        Console.WriteLine("\nGot a request to recover a drone.");
        var possibleDrones = (await _fleet.GetAllAsync()).Where(x => x.DroneId.Equals(record.DroneId)).ToList();
        Console.WriteLine($"Possible drones are: [{string.Join(",", possibleDrones)}]");
        if (!possibleDrones.Any())
        {
            Console.WriteLine($"All my drones are active right now.  Super suspicious..\ndrone -> {record.ToJson()}");
        }
        else if (possibleDrones.Count > 1)
        {
            Console.WriteLine("More than one drone is defined with that ID. Super ambiguous and suspicious...");
        }
        else
        {
            Console.WriteLine($"\nDrone matched {record.DroneId}. Reviving drone.");
            record.State = DroneState.Ready;
            await _fleet.UpdateAsync(record.Update());
            unsuccessful = false;
        }

        return unsuccessful;
    }

    [HttpPost("AssignmentCheck")]
    public async Task AssignmentCheck()
    {
        if (_isInitiatingDrone) return;
        _assignmentLock = true;
        Console.WriteLine("Trying to dequeue some orders...");
        var order = (await GetUnfulfilledOrders()).First();
        // foreach (var order in waitingOrders)
        // {
        while(doNothingWatch.ElapsedMilliseconds < 10000)
        {
            if(!doNothingWatch.IsRunning) doNothingWatch.Start();
        }
        doNothingWatch.Reset();
        if(order.State != OrderState.Waiting) return;
        var first = _assignments.Rotate();
        var current = first;
        var found = false;
        while (!current.Available && !current.Equals(first))
        {
            current =_assignments.Rotate();
            found = current.Available;
        }
        if (!found) return;
        current.OrderId = order.OrderId;
        doNothingWatch.Start();
        await InitiateDelivery(await _fleet.GetByIdAsync(current.DroneId), order);
        // }
        

        /*
        var readyDrones = await GetAvailableDrones();
        foreach (var (order, drone) in waitingOrders.Zip(readyDrones))
        {
            await InitiateDelivery(drone, order);
        }
        _assignmentLock = false;*/
    }

    [HttpPost("AddDrone")]
    public async Task<AddDroneResponse> AddDrone(AddDroneRequest addDroneRequest)
    {
        _isInitiatingDrone = true;
        addDroneRequest.DroneId = BaseEntity.GenerateNewId();

        if ((await _fleet.GetAllAsync())
            .Any(x => x.DroneUrl.Equals(addDroneRequest.DroneUrl) || x.DroneId.Equals(addDroneRequest.DroneId)))
        {
            Console.WriteLine("Either the DroneUrl or DroneId you are trying to use is taken by another drone.");
            return new AddDroneResponse
            {
                DroneId = addDroneRequest.DroneId,
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
            $"\nResponse from _dispatchToSimDroneGateway.InitDrone({initDroneRequest})\n\t->{{DroneId:{initDroneResponse.DroneId},Okay:{initDroneResponse.Okay}}}\n");
        if (!initDroneResponse.Okay)
            return new AddDroneResponse {DroneId = addDroneRequest.DroneId, Success = false};

        var assignFleetRequest = new AssignFleetRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneUrl = addDroneRequest.DroneUrl,
            DispatchUrl = _dispatchUrl,
            HomeLocation = _homeLocation
        };

        Console.WriteLine($"\nProceeding with _dispatchToSimDroneGateway.AssignFleet({assignFleetRequest.ToJson()}\n");
        var assignFleetResponse = _dispatchToSimDroneGateway.AssignFleet(assignFleetRequest).Result;
        var responseString = assignFleetResponse != null
            ? assignFleetResponse.IsInitializedAndAssigned.ToString()
            : "null";
        Console.WriteLine($"\n_dispatchToSimDroneGateway.AssignFleet - response -> {responseString}\n");

        if (assignFleetResponse is {IsInitializedAndAssigned: false})
        {
            Console.WriteLine($"FAILURE! new drone {addDroneRequest.DroneId} was not initiated.");
            return new AddDroneResponse
            {
                DroneId = addDroneRequest.DroneId,
                Success = false
            };
        }

        Console.WriteLine($"\nsuccess! Saving new drone {addDroneRequest.DroneId} to repository.\n");

        var droneRecord = new DroneRecord
        {
            OrderId = "",
            DroneUrl = addDroneRequest.DroneUrl,
            DroneId = addDroneRequest.DroneId,
            HomeLocation = _homeLocation,
            Destination = _homeLocation,
            CurrentLocation = _homeLocation,
            DispatchUrl = _dispatchUrl,
            State = DroneState.Ready
        };

        Console.WriteLine($"\nabout to YEET this drone record:\n{droneRecord.ToJson()}");
        await _fleet.CreateAsync(
            droneRecord);
        _assignments.Enqueue(new Assignment
        {
            DroneId = droneRecord.DroneId,
            OrderId = ""
        });
        _isInitiatingDrone = false;
        return new AddDroneResponse
        {
            DroneId = addDroneRequest.DroneId,
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

    [NonAction]
    private async Task<AssignDeliveryResponse?> InitiateDelivery(DroneRecord drone, Order order)
    {
        if(_assignmentLock) return new AssignDeliveryResponse {DroneId = drone.DroneId, WillDeliverOrder = false};
        _assignmentLock = true;
        var databaseCopy = await _orders.GetByIdAsync(order.OrderId);

        Console.WriteLine($"In DispatchController.InitiateDelivery request\n" +
                          $"Matched order {order.OrderId} with drone {drone.DroneId}, " +
                          $"to location: ({order.DeliveryLocation.Latitude},{order.DeliveryLocation.Longitude})");
        var request = new AssignDeliveryRequest
        {
            DroneId = drone.DroneId,
            OrderId = order.OrderId,
            OrderLocation = order.DeliveryLocation
        };
        var response = await _dispatchToSimDroneGateway.AssignDelivery(request, drone.DroneUrl);
        Debug.Assert(response != null, nameof(response) + " != null");
        if (!response.WillDeliverOrder) return response;
        order.State = OrderState.Assigned;
        order.DroneId = drone.DroneId;
        await _orders.UpdateAsync(order.Update());
        await _dispatchToSimDroneGateway.DeliverOrder(drone.DroneUrl, order.OrderId);
        _assignmentLock = false;
        return response;
    }


    [NonAction]
    private async Task<IEnumerable<Order>> GetUnfulfilledOrders()
    {
        var orders = await _orders.GetAllAsync();
        Console.WriteLine($"All the orders: {orders.Count}");
        var unassignedOrders = orders.Where(o =>
            !o.HasBeenDelivered
            && o is {State: OrderState.Waiting} 
            && string.IsNullOrEmpty(o.DroneId));
        return unassignedOrders;
    }

    [NonAction]
    private async Task<IEnumerable<DroneRecord>> GetAvailableDrones()
    {
        if (_isInitiatingDrone)
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
            var availableDrones = new List<DroneRecord>();
            foreach (var drone in drones)
            {
                if (!await _dispatchToSimDroneGateway.HealthCheck(drone.DroneId)) continue;
                Console.WriteLine($"drone at {drone.DroneUrl} is healthy");
                availableDrones.Add(drone);
            }

            return availableDrones;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<DroneRecord>();
        }
    }

    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse> UpdateDroneStatus(DroneUpdate request)
    {
        var match = new Assignment{DroneId = request.DroneId};
        var first = _assignments.Rotate();
        var current = first;
        while (!current.Equals(match))
        {
            current = _assignments.Rotate();
        }
        current.OrderId = string.Empty;
        while (!_assignments.Peek().Equals(first))
        {
            _assignments.Rotate();
        }
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {request.ToJson()}");
        var updateResult = await _fleet.UpdateAsync(request);
        var response = new UpdateDroneStatusResponse
        {
            DroneId = request.DroneId,
            IsCompletedSuccessfully = updateResult.IsAcknowledged && updateResult.ModifiedCount == 1
        };
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {request.ToJson()}");
        return response;
    }


    [HttpPost("{requestedDroneId:length(24)}")]
    public async Task<ActionResult<DroneRecord>> GetDroneById(string requestedDroneId)
    {
        Console.WriteLine($"DispatchController.Get -> {requestedDroneId}");
        var droneRecord = await _fleet.GetByIdAsync(requestedDroneId);

        if (droneRecord is null) return NotFound();

        return droneRecord;
    }
}