using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Domain.RepositoryDefinitions;

public class Compository : ICompositeRepository
{
    private readonly IMongoCollection<CompositeDocument>? _collection;
    private readonly IMongoCollection<DroneRecord>? _fleet;
    private readonly CompositeDocument _compositeDocument;
    private readonly FilterDefinition<CompositeDocument> _fleetFilter , _ordersFilter, _keysFilter;
    private readonly List<DroneRecord> _droneList;
    private readonly List<Order> _orderList;


    public Compository(IOptions<RepositorySettings> fleetSettings)
    {
        var mongoClient = new MongoClient( fleetSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(fleetSettings.Value.DatabaseName);
        _collection = mongoDatabase.GetCollection<CompositeDocument>( fleetSettings.Value.CollectionName);
        
        _fleet = 
        
        _compositeDocument = _collection.Find(x => true) .FirstOrDefault();
        _fleetFilter = Builders<CompositeDocument>.Filter.Exists("Fleet");
        _ordersFilter = Builders<CompositeDocument>.Filter.Exists("Orders");
        _keysFilter = Builders<CompositeDocument>.Filter.Exists("DronesOrdersMap");
        
        
    }


    public async Task<DronesOrderKey> CreateDroneAsync(DroneRecord newDrone)
    {
        var droneUpdateDefinition = Builders<CompositeDocument>.Update.Push("Fleet", newDrone);
        var keyUpdateDefinition = Builders<CompositeDocument>
                                 .Update
                                 .Push("DronesOrdersMap", newDrone.DroneId)
                                 .Set($"DronesOrdersMap.{newDrone.DroneId}.DroneHasBeenNotified", "false");
        await _collection.FindOneAndUpdateAsync(_fleetFilter, droneUpdateDefinition);
        var map = (await GetDronesOrdersMap());
        map.Add(new DronesOrderKey
        {
            DroneHasBeenNotified = false, 
            DroneId = newDrone.DroneId,
            OrderId = string.Empty
        });

        var s = $"{{(type \\x04) \"DronesOrdersMap\": {string.Join(",", map)}}}";
        Console.WriteLine($"\n\n\n\ns={s}\n\n\n\n");
        BsonArray bsonArray;
        using(var jsonReader = new MongoDB.Bson.IO.JsonReader(s))
        {
            var serializer = new BsonArraySerializer();
            bsonArray = serializer.Deserialize(BsonDeserializationContext.CreateRoot(jsonReader));
        }

        DronesOrderKey key = null;
        foreach(var bsonValue in bsonArray)
        {
            var b = bsonValue.ToBsonDocument();
            key = await _collection.(_keysFilter, b);
        }
        return key;
    }
    
    public async Task<CompositeDocument> TryAssignOrders()
    {
        var allDrones = await GetDrones();
        var allOrders = await GetOrders();
        var keyMap = (await GetDronesOrdersMap()).ToDictionary(x => x.DroneId, x => x.OrderId);
        var unassignedOrderIds = allOrders.Select(x => x.OrderId).Where(orderId => !keyMap.ContainsValue(orderId));
        var availableDrones = allDrones.Select(x => x.DroneId).Where(droneId => keyMap[droneId] == string.Empty);
        var newMap = availableDrones.Zip(unassignedOrderIds) .Select(kvp => new DronesOrderKey { DroneId = kvp.First, OrderId = kvp.Second }).ToList();
        var updateDefinition = Builders<CompositeDocument>.Update.Set("DronesOrderKey", newMap);
        return await _collection.FindOneAndUpdateAsync(k => true, updateDefinition);
    }

    public async Task<BsonDocument> CreateOrderAsync(Order newOrder)
    {
        var orderUpdateDefinition = Builders<CompositeDocument>.Update.Push("Orders", newOrder);
        var updateResult = await _collection.FindOneAndUpdateAsync(_ordersFilter, orderUpdateDefinition);
        var keyValuePairs = await GetAvailableDrones();
        return !(keyValuePairs.Count > 0) 
            ? updateResult.ToBsonDocument() 
            : (await UpdateKeyAsync(keyValuePairs.First() .Key, newOrder.OrderId)).ToBsonDocument();
    }

    public async Task<List<DronesOrderKey>> GetDronesOrdersMap() => Get().DronesOrdersMap;

    public async Task<List<DroneRecord>> GetDrones() => Get()
       .Fleet;


    public async Task<List<Order>> GetOrders() => Get()
       .Orders;


    private CompositeDocument Get() 
        => _collection
        .Find(x => true)
        .FirstOrDefault();


    public async Task<Order> GetOrderByIdAsync(string id)
        => (await GetOrders()).First(x => x.OrderId == id);

    public async Task<Dictionary<string, string>> GetAvailableDrones()
        => (await GetDronesOrdersMap())
          .Where(entry => entry.OrderId == string.Empty)
          .ToDictionary(x => x.DroneId, x => x.OrderId);

    public async Task<DroneRecord> GetDroneByIdAsync(string id)
        => (await GetDrones()).First(x => x.DroneId == id);


    public async Task<UpdateResult> UpdateDroneAsync(UpdateDroneStatusRequest request)
    {
        ;

        var update = Builders<CompositeDocument>
                    .Update.Set($"Fleet.{request.DroneId}.Orders", request.Orders)
                    .Set($"Fleet.{request.DroneId}.State", request.State)
                    .Set($"Fleet.{request.DroneId}.CurrentLocation", request.CurrentLocation)
                    .Set($"Fleet.{request.DroneId}.Destination", request.Destination);

        return await _collection.UpdateOneAsync(_fleetFilter, update, new UpdateOptions {IsUpsert = true});
    }


    public async Task<UpdateResult> UpdateOrderAsync(CompleteOrderRequest request)
    {
        var update = Builders<CompositeDocument>
                    .Update.Set($"Fleet.{request.OrderId}.TimeDelivered", request.Time);

        return await _collection.UpdateOneAsync(_fleetFilter, update, new UpdateOptions {IsUpsert = true});
    }

    public async Task<UpdateResult> UpdateKeyAsync(string droneId, string orderId)
    {
        var update = Builders<CompositeDocument>
                    .Update
                    .Set($"DronesOrdersMap.{droneId}", orderId)
                    .Set($"DronesOrdersMap.HasBeenNotified", false);
        return await _collection.UpdateOneAsync(_keysFilter, update, new UpdateOptions {IsUpsert = true});
    }
    private async Task UpdateRepo()
    {
        _compositeDocument._id = ObjectId.GenerateNewId();
        MongoDB.Driver.FilterDefinition<Domain.Entities.CompositeDocument> filter = new ExpressionFilterDefinition<CompositeDocument>(x => true);
        Domain.Entities.CompositeDocument document = _compositeDocument;

        MongoDB.Driver.FindOneAndReplaceOptions<Domain.Entities.CompositeDocument, Domain.Entities.CompositeDocument> options =
            new FindOneAndReplaceOptions<CompositeDocument>();

        options.IsUpsert = true;
        System.Threading.CancellationToken token = CancellationToken.None;

        if(_collection == null)
        {
            Console.WriteLine("collection is null!");
            return;
        }

        await _collection.FindOneAndReplaceAsync(filter, _compositeDocument, options, token);
    }


    public async Task<UpdateResult> EnqueueOrder(EnqueueOrderRequest request)
    {
        var update = Builders<CompositeDocument>.Update.Push($"Fleet.Orders", request.Order);
        var filter = Builders<CompositeDocument>.Filter.Where(d => d.Fleet.Any(x => x.State == DroneState.Ready));
        return await _collection.UpdateOneAsync(filter, update);
    }


    public async Task<UpdateResult> AssignOrder(string orderId, string droneId)
    {
        var order = await GetOrderByIdAsync(orderId);
        var drone = await GetDroneByIdAsync(droneId);
        drone.Orders.Add(order);
        await RemoveOrderAsync(orderId);

        await UpdateDroneAsync(new UpdateDroneStatusRequest
        {
            CurrentLocation = drone.CurrentLocation,
            Destination = drone.Destination,
            DroneId = droneId,
            Orders = drone.Orders
        });

        var filter = Builders<CompositeDocument>.Filter.Eq($"Fleet.0.{{'DroneId':1}}", droneId);
        var update = Builders<CompositeDocument>.Update.Push($"Fleet.0.{droneId}.Orders", order);
        return await _collection.UpdateOneAsync(filter, update);
    }


    public Task<DeleteResult> RemoveDroneAsync(string id)
    {
        var filter = Builders<CompositeDocument>.Filter.Eq("Fleet", new BsonObjectId(ObjectId.Parse(id)));
        return _collection.DeleteOneAsync(filter);
    }


    public async Task<Order> RemoveOrderAsync(string id)
    {
        var doc = Get();
        var order = doc.Orders.FirstOrDefault(x => x.OrderId == id);

        doc.Orders = doc.Orders.Where(x => x.OrderId != id)
                        .ToList();

        await UpdateRepo();
        return order;
    }


    public async Task<Task<CompositeDocument>> RemoveOrderAsync(Order order)
    {
        var update = Builders<CompositeDocument>.Update.Pull("Orders", order);
        return _collection.FindOneAndUpdateAsync(_fleetFilter, update);
    }


    public override string ToString()
    {
        return _compositeDocument.ToString();
    }
}
