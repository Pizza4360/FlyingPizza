using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
    
    private const int RefreshInterval = 2000;
    
    
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
        
        _fleet =  mongoDatabase.GetCollection<DroneRecord>(repoSettings.Value.Fleet);
        _fleetFilter = Builders<DroneRecord>.Filter.Exists("FleetCompositeTest");
        
        _orders =  mongoDatabase.GetCollection<Order>(repoSettings.Value.Orders);
        _ordersFilter = Builders<Order>.Filter.Exists("OrdersCompositeTest");
        
        _assignments =  mongoDatabase.GetCollection<Assignment>(repoSettings.Value.Assignments);
        _assignmentsFilter = Builders<Assignment>.Filter.Exists("Assignments");
        TryAssignOrders();

        _timer = new Timer(TryAssignmentsCallback, null, 0, RefreshInterval);
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public async Task CreateDroneAsync(DroneRecord newDrone)
    {
        await _fleet.InsertOneAsync(newDrone);
        var assignment = new Assignment {DroneId = newDrone.DroneId, OrderId = string.Empty, ShouldNotifyDrone = true};
        await _assignments.InsertOneAsync(assignment);
    }

    public async Task<Order> EnqueueOrder(Order newOrder)
    {
        await _orders.InsertOneAsync(newOrder);
        return newOrder;
    }

    public async Task<List<DroneRecord>> GetDrones() => _fleet.Find(_ => true).ToList();
    public async Task<List<Order>> GetOrders() => _orders.Find(_ => true).ToList();
    public async Task<List<Assignment>> GetAssignments() => _assignments.Find(_ => true).ToList();


    public async Task<Order> GetOrderByIdAsync(string id)
        => (await GetOrders()).First(x => x.OrderId.Equals(id));


    public async Task<List<string>> GetAvailableDrones()
    {
        var assignments = await GetAssignments();
        return (from assignment in assignments where assignment.OrderId.Equals(string.Empty) select assignment.DroneId).ToList();
    }


    public async Task<DroneRecord> GetDroneByIdAsync(string id)
        => (await GetDrones()).First(x => x.DroneId.Equals(id));


    public async Task<UpdateResult> UpdateDroneAsync(UpdateDroneStatusRequest request)
    {
        var update = Builders<DroneRecord>
                    .Update
                    .Set($"Fleet.{request.DroneId}.Orders", request.Orders)
                    .Set($"Fleet.{request.DroneId}.State", request.State)
                    .Set($"Fleet.{request.DroneId}.CurrentLocation", request.CurrentLocation)
                    .Set($"Fleet.{request.DroneId}.Destination", request.Destination);
        return await _fleet.UpdateOneAsync(_fleetFilter, update, new UpdateOptions {IsUpsert = true});
    }


    public async Task<UpdateResult> UpdateOrderAsync(CompleteOrderRequest request)
    {
        var update = Builders<Order>
                    .Update.Set($"Fleet.{request.OrderId}.TimeDelivered", request.Time);
        return await _orders.UpdateOneAsync(_ordersFilter, update, new UpdateOptions {IsUpsert = true});
    }

    public async Task<UpdateResult> UpdateAssinmentAsync(string droneId, string orderId, bool ShouldBeNotified)
    {
        var update = Builders<Assignment>
                    .Update
                    .Set($"DronesOrdersMap.{droneId}", orderId)
                    .Set($"DronesOrdersMap.HasBeenNotified", ShouldBeNotified);
        return await _assignments.UpdateOneAsync(_assignmentsFilter, update, new UpdateOptions {IsUpsert = true});
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

    public async Task<Order> RemoveAssignment(Order order)
    {
        var deleteOptions = new FindOneAndDeleteOptions<Order>();
        return await _orders.FindOneAndDeleteAsync(_ordersFilter, deleteOptions, CancellationToken.None);
    }

    public async Task TryAssignOrders()
    {
        Console.WriteLine("trying to assign some orders...");
        var allOrders = await GetOrders();
        var allAssignments = await GetAssignments();
        var unassignedOrderIds = GetUnassignedOrderIds(allOrders, allAssignments);
        var availableDronesIds = await GetAvailableDrones();
        var newAssignments = GetNewAssignments(availableDronesIds, unassignedOrderIds);

        foreach(var updateDefinition in newAssignments.Select(newAssignment => Builders<Assignment>.Update.Set("OrderId", newAssignment.OrderId)))
        {
            await _assignments.FindOneAndUpdateAsync(k => true, updateDefinition);
        }
    }
    private static List<Assignment> GetNewAssignments(IEnumerable<string> availableDronesIds, IEnumerable<string> unassignedOrderIds)
    {
        return availableDronesIds
              .Zip(unassignedOrderIds)
              .Select(kvp => 
                   new Assignment { DroneId = kvp.First, OrderId = kvp.Second })
              .ToList();
    }


    private static IEnumerable<string> GetUnassignedOrderIds(IEnumerable<Order> allOrders, List<Assignment> keyMap)
    {
        return allOrders.Select(x => x.OrderId).Where(orderId => keyMap.TrueForAll(y => !y.OrderId.Equals(orderId)));
    }

}
