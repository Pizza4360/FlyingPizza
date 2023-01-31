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

public class DeliveryRepository : IDeliveriesRepository
{
    private readonly IMongoCollection<DeliveryEntity> _collection;

    public DeliveryRepository(RepositorySettings settings) {
        var mongoClient = new MongoClient(settings.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);

        _collection = mongoDatabase.GetCollection<DeliveryEntity>(
            settings.DatabaseName);
        
        Console.WriteLine($"this should be 'Deliveries'"
                          + $">>>{settings.Collection}<<<");
    }

    public DeliveryRepository(IMongoCollection<DeliveryEntity> getCollection)
    {
        _collection = getCollection;
    }

    public async Task CreateAsync(DeliveryEntity newDeliveryEntity)
    {
        Console.WriteLine($"creating:\n{newDeliveryEntity.ToJson()}");
        
        await _collection.InsertOneAsync(newDeliveryEntity);
    }

    public async Task<DeliveryEntity> GetByIdAsync(string id)
    {
        return await _collection
            .Find(x => x.DeliveryId == id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveAsync(string id)
    {
        return (await _collection
                .DeleteOneAsync(x => x.DroneId == id)
        ).IsAcknowledged;
    }


    public async Task<UpdateResult> UpdateAsync(DeliveryUpdate update) 
    {
        Console.WriteLine($"DeliveryRepository.UpdateAsync() -> "
                          + $"{update.ToJson()}\n(status = {update.Status})");
        
        var filter = Builders<DeliveryEntity>.Filter
            .Eq(o => o.DeliveryId, update.DeliveryId);
        
        var definition = Builders<DeliveryEntity>.Update
            .Set(o => o.DroneId, update.DroneId)
            .Set(o => o.Status, update.Status)
            .Set(o => o.TimeDelivered, update.TimeDelivered)
            .Set(o => o.HasBeenDelivered, update.HasBeenDelivered);
        
        return await _collection.UpdateOneAsync(
            filter,
            definition,
            new UpdateOptions {IsUpsert = false}
        );
    }

    public Task<bool> Update(DeliveryEntity deliveryEntity){ 
        
        var result = _collection.ReplaceOneAsync(
            filter: new BsonDocument("_id", deliveryEntity.DroneId),
            options: new ReplaceOptions {IsUpsert = true},
            replacement: deliveryEntity
         );
        
        result.Wait();
        
        return Task.FromResult(result.IsCompletedSuccessfully);
    }

    public Task UpdateAsync(string id, 
                            DeliveryUpdate update)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync<T>(string id, T update) 
    {
        throw new NotImplementedException();
    }

    public async Task<List<DeliveryEntity>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }
}