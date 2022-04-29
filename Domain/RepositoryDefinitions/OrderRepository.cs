using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;
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
    }

    public async Task CreateAsync(Order newOrder)
    {
        await _collection.InsertOneAsync(newOrder);
    }

    public async Task<Order> GetByIdAsync(string id) =>
        await _collection.Find(x => x.OrderId == id).FirstOrDefaultAsync();

    public async Task<IEnumerable<Order>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<bool> RemoveAsync(string id) =>
        (await _collection.DeleteOneAsync(x => x.DroneId == id)).IsAcknowledged;

    public async Task<bool> UpdateOrderCompletionTime(string orderId, DateTime deliveryTime)
    {
        var updateDefinition = new UpdateDefinitionBuilder<Order>()
            .Set(record => record.TimeDelivered, deliveryTime);
        var result = await _collection.UpdateOneAsync(record => record.OrderId == orderId, updateDefinition);
        return result.IsAcknowledged;
    }
}