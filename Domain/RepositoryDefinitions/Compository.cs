using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public class Compository : ICompositeRepository
{
    private readonly IMongoCollection<DroneRecord>? _fleet;
    private readonly IMongoCollection<Order> _orders;
    private readonly IMongoCollection<Assignment> _assignments;

    private FilterDefinition<DroneRecord> _fleetFilter;
    private FilterDefinition<Order> _ordersFilter;
    private FilterDefinition<Assignment> _assignmentsFilter;
    private readonly List<DroneRecord> _droneList;
    private readonly List<Order> _orderList;

    private const int RefreshInterval = 10000;


    private Stopwatch _stopwatch;
    private Timer _timer;
    private async void TryAssignmentsCallback(object _) => await TimerCheck();


    private async Task TimerCheck()
    {
        if(_stopwatch.ElapsedMilliseconds > RefreshInterval)
        {
            _stopwatch.Stop();
            _stopwatch.Reset();
            TryAssignOrders();
            _stopwatch.Start();
        }
    }


    public Compository(IOptions<RepositorySettings> repoSettings)
    {
        var mongoClient = new MongoClient(repoSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(repoSettings.Value.DatabaseName);

        _fleet = mongoDatabase.GetCollection<DroneRecord>(repoSettings.Value.Fleet);
        _fleetFilter = Builders<DroneRecord>.Filter.Exists("FleetCompositeTest");

        _orders = mongoDatabase.GetCollection<Order>(repoSettings.Value.Orders);
        _ordersFilter = Builders<Order>.Filter.Exists("OrdersCompositeTest");

        _assignments = mongoDatabase.GetCollection<Assignment>(repoSettings.Value.Assignments);
        _assignmentsFilter = Builders<Assignment>.Filter.Exists("Assignments");
        TryAssignOrders();

        _timer = new Timer(TryAssignmentsCallback, null, 0, RefreshInterval);
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }


    public async Task<Tuple<DroneRecord, Assignment>> CreateDroneAsync(DroneRecord @newDrone)
    {
       await _fleet.InsertOneAsync(newDrone);
       var assignment = new Assignment {DroneId = newDrone.DroneId, OrderId = string.Empty, ShouldNotifyDrone = true};
       await _assignments.InsertOneAsync(assignment);
       return new Tuple<DroneRecord, Assignment>(newDrone, assignment);
    }


    public async Task<Order> EnqueueOrder(Order newOrder)
    {
        Console.WriteLine($"Adding order {newOrder.ToJson()}");
        var task = _orders.InsertOneAsync(newOrder);
        task.Wait();
        newOrder = await GetOrderByIdAsync(newOrder.OrderId);
        return newOrder;
    }


    public async Task<List<DroneRecord>> GetDrones() => (await _fleet.FindAsync(_ => true))
                                                              .ToList();


    public async Task<List<Order>> GetOrders() => _orders.Find(_ => true)
                                                         .ToList();


    public async Task<List<Assignment>> GetAssignments() => _assignments.Find(_ => true)
                                                                        .ToList();


    public async Task<Order> GetOrderByIdAsync(string id)
    {
        return (await _orders.FindAsync(Builders<Order>.Filter.Eq("OrderId", id))).FirstOrDefault();
    }


    public async Task<List<string>> GetAvailableDroneIds()
    {
        var assignments = await GetAssignments();

        return (await GetNewAssignments()).Select(x => x.DroneId)
                                          .ToList();
    }


    public async Task<List<Assignment>> GetNewAssignments()
    {
        var assignments = await GetAssignments();
        return (from assignment in assignments where assignment.OrderId.Equals(string.Empty) select assignment).ToList();
    }


    public async Task<DroneRecord> GetDroneByIdAsync(string id)
    {
        (await _fleet.FindAsync(x => id.Equals(x.
    }


    public async Task<UpdateResult> UpdateDroneAsync(UpdateDroneStatusRequest request)
    {
        Console.WriteLine("updating drone status");
        var update = Builders<DroneRecord>
                    .Update
                    .Set($"State", request.State)
                    .Set($"CurrentLocation", request.CurrentLocation)
                    .Set($"Destination", request.Destination);
        var filter = Builders<DroneRecord>.Filter.Eq("DroneId", request.DroneId);
        var result = await _fleet.UpdateOneAsync(filter, update, new UpdateOptions {IsUpsert = false});
        // Console.WriteLine($"\n\n\n\nUpdate request: {request.ToJson()}\n\nUpdated result: {result.ToJson()}\n");
        return result;
    }


    public async Task<UpdateResult> UpdateOrderAsync(CompleteOrderRequest request)
    {
        var update = Builders<Order>
                    .Update.Set($"TimeDelivered", request.Time);

        return await _orders.UpdateOneAsync(Builders<Order>.Filter.Eq("OrderId", request.OrderId), update, new UpdateOptions {IsUpsert = true});
    }


    public async Task UpdateAssignmentAsync(string droneId, bool shouldBeNotified)
    {
        var assignment =
        (
            from a in await GetAssignments()
            where a.DroneId.Equals(droneId) select a).First();

        var filter = Builders<Assignment>.Filter.Eq("DroneId", droneId);
        var update = Builders<Assignment>
                    .Update
                    .Set($"OrderId", assignment.OrderId)
                    .Set($"HasBeenNotified", shouldBeNotified);
        await _assignments.UpdateOneAsync(filter, update, new UpdateOptions {IsUpsert = true});
    }


    public Task<DeleteResult> RemoveDroneAsync(string id)
    {
        var filter = Builders<DroneRecord>.Filter.Eq("Fleet", new BsonObjectId(ObjectId.Parse(id)));
        return _fleet.DeleteOneAsync(filter);
    }


    public async Task<DeleteResult> RemoveOrderAsync(string id)
    {
        var filter = Builders<Order>.Filter.Eq("Order", new BsonObjectId(ObjectId.Parse(id)));
        return await _orders.DeleteOneAsync(filter);
    }


    public async Task RemoveAssignment(Assignment assignment)
    {
        var deleteOptions = new FindOneAndDeleteOptions<Assignment>();
        await _assignments.FindOneAndDeleteAsync<Assignment>(x => x.DroneId.Equals(assignment.DroneId), deleteOptions, CancellationToken.None);
    }


    public async Task TryAssignOrders()
    {
        Console.WriteLine("trying to assign some orders...");
        var allOrders = await GetOrders();
        var allAssignments = await GetAssignments();
        var unassignedOrderIds = GetUnassignedOrderIds(allOrders, allAssignments);
        var availableDronesIds = await GetAvailableDroneIds();
        var newAssignments = GetNewAssignments(availableDronesIds, unassignedOrderIds);

        foreach(var updateDefinition in (await newAssignments).Select(newAssignment => Builders<Assignment>.Update.Set("OrderId", newAssignment.OrderId)))
        {
            await _assignments.FindOneAndUpdateAsync(k => true, updateDefinition);
        }

        foreach(var assignment in allAssignments)
        {
            if((await GetDrones()).TrueForAll(x => !x.DroneId.Equals(assignment.DroneId)))
            {
                RemoveAssignment(assignment);
            }
        }
    }


    private static async Task<List<Assignment>> GetNewAssignments(IEnumerable<string> availableDronesIds, IEnumerable<string> unassignedOrderIds)
    {
        return availableDronesIds
              .Zip(unassignedOrderIds)
              .Select(kvp =>
                   new Assignment {DroneId = kvp.First, OrderId = kvp.Second})
              .ToList();
    }


    private static IEnumerable<string> GetUnassignedOrderIds(IEnumerable<Order> allOrders, List<Assignment> keyMap)
    {
        return allOrders.Select(x => x.OrderId)
                        .Where(orderId => keyMap.TrueForAll(y => !y.OrderId.Equals(orderId)));
    }


    public async Task<IEnumerable<AssignDeliveryRequest>> GenerateDeliveryRequests()
    {
        var allAssignmentsEnum = await GetAssignments();

        var newAssignments = 
            from a in allAssignmentsEnum
            where !a.OrderId.Equals(string.Empty) && a.ShouldNotifyDrone 
            select a;
        var assignments = newAssignments as Assignment[] ?? newAssignments.ToArray();
        Console.WriteLine($"newAssignments:{assignments}");

        var newOrderIds = 
            from a in assignments
            select a.OrderId;
        var orderIds = newOrderIds as string[] ?? newOrderIds.ToArray();
        Console.WriteLine($"newOrderIds:{orderIds.Length}");

        
        var newOrders = 
            from o in await GetOrders() 
            from a in allAssignmentsEnum
            where orderIds.Contains(o.OrderId) && allAssignmentsEnum.Select(x=>x.OrderId).Contains(o.OrderId)
            select o;
        var newOrdersEnum = newOrders as Order[] ?? newOrders.ToArray();
        Console.WriteLine($"newOrders:{newOrdersEnum.Length}");

        var newDrones = 
            from d in await GetDrones() 
            where  assignments.Select(x => x.DroneId).Contains(d.DroneId) 
            select d;
        
        var droneRecords = newDrones as DroneRecord[] ?? newDrones.ToArray();
        Console.WriteLine($"newDrones:{droneRecords.Length}");

        var newRequests =
            from a in allAssignmentsEnum
            from o in newOrdersEnum
            from d in newDrones
            where o.OrderId.Equals(a.OrderId) && d.DroneId.Equals(a.DroneId)
            select new AssignDeliveryRequest {DroneUrl = d.DroneUrl, Order = o, DroneId = d.DroneId};
        Console.WriteLine($"newRequests:{newRequests.ToArray().Length}");
        return newRequests;
    }
}
