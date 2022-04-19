﻿using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.InterfaceDefinitions.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.InterfaceImplementations.Repositories;

public class OrderRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _collection;
    public OrderRepository(IOptions<OrdersDatabaseSettings> ordersSettings) //: Domain.InterfaceDefinitions.Repositories
    {
        var mongoClient = new MongoClient(
            ordersSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            ordersSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<Order>(
            ordersSettings.Value.CollectionName);
        Console.WriteLine($"this should be 'Orders'>>>{ordersSettings.Value.CollectionName}<<<");
    }

    public async Task CreateAsync(Order newOrder)
    {
        await _collection.InsertOneAsync(newOrder);
    }

    public async Task<Order> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

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

    public async Task<bool> RemoveAsync(string id) =>
        (await _collection.DeleteOneAsync(x => x.Id == id)).IsAcknowledged;


    public async Task<bool> UpdateAsync(Order order)
    {
        var result = await _collection.UpdateOneAsync(
            record => record.Id == order.Id,
            GetUpdateDefinition(order));
        return result.IsAcknowledged;
    }

    private static UpdateDefinition<Order> GetUpdateDefinition(Order order)
    {
        var builder = new UpdateDefinitionBuilder<Order>();
        UpdateDefinition<Order> updateDefinition = null;
        foreach (var property in order.GetType().GetProperties())
        {
            if (property != null)
            {
                if (updateDefinition == null)
                {
                    updateDefinition = builder.Set(property.Name, property.GetValue(order));
                }
                else
                {
                    updateDefinition = updateDefinition.Set(property.Name, property.GetValue(order));
                }
            }
        }

        return updateDefinition;
    }
}