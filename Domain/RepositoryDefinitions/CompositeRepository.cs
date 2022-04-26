using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public class CompositeRepository : ICompositeRepository
{
    private readonly IMongoCollection<CompositeDocument>? _collection;
    private readonly CompositeDocument _compositeDocument;
    private readonly FilterDefinition<CompositeDocument> _IdFilter;

    public CompositeRepository(IOptions<RepositorySettings> fleetSettings) 
    {
        var mongoClient = new MongoClient(
            fleetSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            fleetSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<CompositeDocument>(
            fleetSettings.Value.CollectionName);

        _compositeDocument = _collection.Find(x => true).FirstOrDefault();
        _IdFilter = Builders<CompositeDocument>.Filter.Eq("_id",  BsonObjectId.Create(_compositeDocument._id));
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


    public async Task<List<DroneRecord>> GetDrones() => Get() .Fleet;


    public async Task<List<Order>> GetOrders() => Get()
       .Orders;


    private CompositeDocument Get() => _collection.Find(x => true)
                                                  .FirstOrDefault();


    public async Task<Order> GetOrderByIdAsync(string id)
        => (await GetOrders()).First(x => x.OrderId == id);


    public async Task<DroneRecord> GetDroneByIdAsync(string id)
        => (await GetDrones()).First(x => x.DroneId == id);


    public Task<DeleteResult> RemoveDroneAsync(string id)
    {
        var filter = Builders<CompositeDocument>.Filter.Eq("Fleet", new BsonObjectId(ObjectId.Parse(id)));
        return _collection.DeleteOneAsync(filter);
    }


    public async Task<Order> RemoveOrderAsync(string id)
    {
        var doc = Get();
        var order = doc.Orders.FirstOrDefault(x => x.OrderId == id);
        doc.Orders = doc.Orders.Where(x => x.OrderId != id).ToList();
        await UpdateRepo();
        return order;
    }


    public async Task<UpdateResult> EnqueuOrder(EnqueueOrderRequest request)
    {
        var update = Builders<CompositeDocument>.Update .Push("Fleet.Orders", request.Order);
        var filter = Builders<CompositeDocument>.Filter.Where(d => d.Fleet.Any(x => x.State == DroneState.Ready));
        return await _collection.UpdateOneAsync(filter, update);
    }
    public async Task<UpdateResult> UpdateDroneAsync(UpdateDroneStatusRequest request)
    {
 ;
        var update = Builders<CompositeDocument>
                    .Update.Set($"Fleet.{request.DroneId}.Orders", request.Orders)
                    .Set($"Fleet.{request.DroneId}.State", request.State)
                    .Set($"Fleet.{request.DroneId}.CurrentLocation", request.CurrentLocation)
                    .Set($"Fleet.{request.DroneId}.Destination", request.Destination);
        return await _collection.UpdateOneAsync(_IdFilter, update, new UpdateOptions {IsUpsert = true});
    }


    public async Task<UpdateResult> UpdateOrderAsync(CompleteOrderRequest request)
    {
        var update = Builders<CompositeDocument>
                    .Update.Set($"Fleet.{request.OrderId}.TimeDelivered", request.Time);
        return await _collection.UpdateOneAsync(_IdFilter, update, new UpdateOptions {IsUpsert = true});
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
        await _collection.FindOneAndReplaceAsync(filter, _compositeDocument,options, token);
    }


    public async Task<Task<CompositeDocument>> CreateDroneAsync(DroneRecord newDrone)
    {
        var update = Builders<CompositeDocument>.Update.Push("Fleet", newDrone);
        return _collection.FindOneAndUpdateAsync(_IdFilter, update);
    }


    public async Task<Task<CompositeDocument>> CreateOrderAsync(Order newOrder)
    {
        var update = Builders<CompositeDocument>.Update.Push("Orders", newOrder);
        return _collection.FindOneAndUpdateAsync(_IdFilter, update);
    }


    public async Task<Task<CompositeDocument>> RemoveOrderAsync(Order order)
    {
        var update = Builders<CompositeDocument>.Update.Pull("Orders", order);
        return _collection.FindOneAndUpdateAsync(_IdFilter, update);
    }


    public override string ToString()
    {
        return _compositeDocument.ToString();
    }
}
