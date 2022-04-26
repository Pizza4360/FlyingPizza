using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public class CompositeRepository : ICompositeRepository
{
    private readonly IMongoCollection<CompositeDocument>? _collection;
    private readonly CompositeDocument _compositeDocument;


    public CompositeRepository()
    {
        var mongoClient = new MongoClient("mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
        var mongoDatabase = mongoClient.GetDatabase("Capstone");
        _collection = mongoDatabase.GetCollection<CompositeDocument>("CompositeTest");

        _compositeDocument = _collection.Find(x => true)
                                        .FirstOrDefault();
    }


    public async Task<bool> AssignOrder(string orderId, string droneId)
    {
        var drone = _compositeDocument.Fleet.First(x => x.DroneId == droneId);
        var order = await RemoveOrderAsync(orderId);
        if(drone != null) drone.Orders.Add(order);
        else
        {
            return false;
        }
        await UpdateDroneAsync(drone);
        return true;
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


    public async Task<bool> RemoveDroneAsync(string id)
    {
        var doc = Get();

        doc.Fleet = doc.Fleet.Where(x => x.DroneId != id)
                       .ToList();

        await UpdateRepo();
        return true;
    }


    public async Task<Order> RemoveOrderAsync(string id)
    {
        var doc = Get();
        var order = doc.Orders.FirstOrDefault(x => x.OrderId == id);
        doc.Orders = doc.Orders.Where(x => x.OrderId != id).ToList();
        await UpdateRepo();
        return order;
    }


    public async Task<bool> UpdateDroneAsync(DroneRecord entity)
    {
        var record = Get()
                    .Fleet.First(x => x.DroneId == entity.DroneId);
        var index = _compositeDocument.Fleet.IndexOf(record);

        if(index == -1)
            return false;
        Console.WriteLine("\n\n\n\n" + record);

        record.CurrentLocation = record.CurrentLocation == null ? record.CurrentLocation : entity.CurrentLocation;
        record.DroneId = record.DroneId == null ? record.DroneId : entity.DroneId;
        record.Destination = record.Destination == null ? record.Destination : entity.Destination;
        record.DispatchUrl = record.DispatchUrl == null ? record.DispatchUrl : entity.DispatchUrl;
        record.BadgeNumber = record.BadgeNumber == null ? record.BadgeNumber : entity.BadgeNumber;
        record.DroneUrl = record.DroneUrl == null ? record.DroneUrl : entity.DispatchUrl;
        record.HomeLocation = record.HomeLocation == null ? record.HomeLocation : entity.HomeLocation;
        record.Orders = record.Orders == null ? record.Orders : entity.Orders;
        _compositeDocument.Fleet[index] = record;
        Console.WriteLine("\n\n\n\n" + record);
        await UpdateRepo();
        return true;
    }


    public async Task<bool> UpdateOrderAsync(Order entity)
    {
        var record = Get()
                    .Orders.First(x => x.OrderId == entity.OrderId);

        record.TimeDelivered = entity.TimeDelivered;
        await UpdateRepo();
        return true;
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


    public async Task CreateDroneAsync(DroneRecord newDrone)
    {
        _compositeDocument.Fleet.Add(newDrone);
        await UpdateRepo();
    }


    public async Task CreateOrderAsync(Order newOrder)
    {
        _compositeDocument.Orders.Add(newOrder);
        await UpdateRepo();
    }


    public async Task<bool> Update(Order order)
    {
        foreach(
            var oldOrder
            in _compositeDocument
              .Orders
              .Where(oldOrder => order.OrderId == oldOrder.OrderId))
        {
            oldOrder.TimeDelivered = order.TimeDelivered;
            break;
        }

        await UpdateRepo();
        return true;
    }


    public async Task<bool> RemoveOrderAsync(Order order)
    {
        _compositeDocument.Orders.Remove(order);
        await UpdateRepo();
        return true;
    }


    public override string ToString()
    {
        return _compositeDocument.ToString();
    }
}
