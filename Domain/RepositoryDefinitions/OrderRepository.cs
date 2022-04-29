using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseAccess;

public class OrderRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _collection;
    public OrderRepository(IOptions<OrdersDatabaseSettings>? ordersSettings) //: Domain.InterfaceDefinitions.Repositories
    {
        var mongoClient = new MongoClient(
            ordersSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            ordersSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<Order>(
            ordersSettings.Value.CollectionName);
        Console.WriteLine($"this should be 'Orders'>>>{ordersSettings.Value.CollectionName}<<<");
    }
    public OrderRepository(OrdersDatabaseSettings settings) //: Domain.InterfaceDefinitions.Repositories
    {
        var mongoClient = new MongoClient(
            settings.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.DatabaseName);

        _collection = mongoDatabase.GetCollection<Order>(
            settings.CollectionName);
        Console.WriteLine($"this should be 'Orders'>>>{settings.CollectionName}<<<");
    }
    public async Task CreateAsync(Order newOrder)
    {
        await _collection.InsertOneAsync(newOrder);
    }

    public async Task<Order> GetByIdAsync(string id) =>
        await _collection.Find(x => x.DroneId == id).FirstOrDefaultAsync();

    public async Task<List<Order>> GetAllAsync() => (await _collection.FindAsync(_ => true)).ToList();

    public Task<bool> 
        Update(Order order)
    {
        var result = _collection.ReplaceOneAsync(
            new BsonDocument("_id", order.DroneId),
            options: new ReplaceOptions { IsUpsert = true },
            replacement: order);
        result.Wait();
        return Task.FromResult<bool>(result.IsCompletedSuccessfully);
    }

    public async Task<bool> RemoveAsync(string id) =>
        (await _collection.DeleteOneAsync(x => x.DroneId == id)).IsAcknowledged;
    

    public async Task<UpdateResult> UpdateAsync(Order order)
    {
        var update = Builders<Order>
            .Update.Set(o => o.TimeDelivered, order.TimeDelivered);

        return await _collection.UpdateOneAsync(Builders<Order>.Filter.Eq(o => o.Id.Equals(order.Id), true), update, new UpdateOptions {IsUpsert = false});
    }

    public async Task UpdateAsync(string requestOrderId, OrderState state)
    {
        var order = await GetByIdAsync(requestOrderId);
        var filterDefinition = Builders<Order>.Filter.Eq(o => o.Id.Equals(order.Id), true);
        var update = Builders<Order>
            .Update.Set(o => o.State, state);
        await _collection.UpdateOneAsync(filterDefinition, update, new UpdateOptions {IsUpsert = false});
    }
}