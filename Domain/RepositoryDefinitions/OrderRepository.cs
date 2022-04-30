using System;
using System.Collections.Generic;
using System.Linq;
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
    public OrderRepository(RepositorySettings settings) //: Domain.InterfaceDefinitions.Repositories
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
        await _collection.Find(x => x.OrderId == id).FirstOrDefaultAsync();
    
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

    public Task UpdateAsync(string id, OrderUpdate update)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync<T>(string id, T update)
    {
        throw new NotImplementedException();
    }


    public async Task<UpdateResult> UpdateAsync(OrderUpdate update)
    {
        Console.WriteLine($"OrderRepository.UpdateAsync() -> {update.ToJson()}\n(state = {update.State})");
        var filter = Builders<Order>.Filter
            .Eq(o => o.OrderId, update.OrderId);
        var definition = Builders<Order>.Update
            .Set(o => o.DroneId, update.DroneId)
            .Set(o => o.State, update.State)
            .Set(o => o.TimeDelivered, update.TimeDelivered)
            .Set(o => o.HasBeenDelivered, update.HasBeenDelivered);
        return await _collection.UpdateOneAsync(filter, definition, new UpdateOptions {IsUpsert = false});
    }
}