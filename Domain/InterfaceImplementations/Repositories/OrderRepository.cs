using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.InterfaceImplementations.Repositories;

public class OrderRepository
{
    private readonly IMongoCollection<Order> _collection;
    public OrderRepository(IOptions<DatabaseSettings> ordersSettings) //: Domain.InterfaceDefinitions.Repositories
    {
        var mongoClient = new MongoClient(
            ordersSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            ordersSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<Order>(
            ordersSettings.Value.CollectionName);
        Console.WriteLine($"this should be 'Orders'>>>{ordersSettings.Value.CollectionName}<<<");
    }
    
    public OrderRepository(string connectionString, string databaseName, string collctionName) //: Domain.InterfaceDefinitions.Repositories
    {
        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseName);
        _collection = mongoDatabase.GetCollection<Order>(collctionName);
    }

    public async Task<Order?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<string?> CreateAsync(Order newOrder)
    {
        var updateDefinition = Builders<Order>.Update
                .Set(order => order.BadgeNumber, newOrder.BadgeNumber)
                .Set(order => order.Id, newOrder.Id)
                .Set(order => order.CustomerName, newOrder.CustomerName)
                .Set(order => order.DeliveryAddress, newOrder.DeliveryAddress)
                .Set(order => order.Items, newOrder.Items)
                .Set(order => order.DeliveryLocation, newOrder.DeliveryLocation)
                .Set(order => order.TimeDelivered, newOrder.TimeDelivered)
                .Set(order => order.TimeOrdered, newOrder.TimeOrdered)
                .Set(order => order.HasBeenDelivered, newOrder.HasBeenDelivered);
            return _collection.UpdateOneAsync(_ => false, updateDefinition, new UpdateOptions { IsUpsert = true }).Result.UpsertedId.ToString();
    }

    public async Task<Order> 
        GetByIdAsync(string id) 
        => _collection.FindAsync(x => x.Id.Equals(id)).Result.First();


    public Task<IEnumerable<Order>> 
        GetByIdsAsync(IEnumerable<string> ids) 
        => Task.FromResult<IEnumerable<Order>>(ids.Select(id => GetByIdAsync(id).Result).ToList());

    public Task<bool> 
        Delete(string id)
    {
        
        return Task.FromResult(_collection.DeleteOne(x => x.Id.Equals(id)).IsAcknowledged);
    }

    public Task<bool> 
        Update(Order order)
    {
        var result = _collection.ReplaceOneAsync(
            new BsonDocument("_id", order.Id),
            options: new ReplaceOptions { IsUpsert = true },
            replacement: order);
        result.Wait();
        return Task.FromResult(result.IsCompletedSuccessfully);
    }

    //https://stackoverflow.com/questions/40526758/patch-rest-api-to-partial-update-mongodb-in-net
    // https://mongodb-entities.com/wiki/Code-Samples.html
    public async Task<bool> 
        PatchTimeCompleted(string id)
    {
        var doc = JsonDocument.Parse($"{{\"TimeDelivered\":\"{DateTime.Now}\"}}").ToBsonDocument();
        var updateDefinition = new BsonDocumentUpdateDefinition<Order>(new BsonDocument("$set", doc));
        var options = new UpdateOptions();
        var token = CancellationToken.None;
        var result = await _collection.UpdateOneAsync( Builders<Order>.Filter.Eq("_id", id), updateDefinition,options, token);
        return result.IsAcknowledged;
    }
}