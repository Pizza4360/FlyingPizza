using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using MongoDB.Driver;

namespace DatabaseAccess;

public class DroneRepository : IDroneRepository
{
    private readonly IMongoCollection<DroneEntity> _collection;

    public DroneRepository(RepositorySettings settings) 
    {
        var mongoClient = new MongoClient(
            settings.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.DatabaseName);

        _collection = mongoDatabase.GetCollection<DroneEntity>(
            settings.DatabaseName);
        
        Console.WriteLine($"this should be 'Fleet'>>>"
                          + $"{settings.DatabaseName}<<<");
    }

    public DroneRepository(IMongoCollection<DroneEntity> getCollection)
    {
        _collection = getCollection;
    }


    public async Task CreateAsync(DroneEntity drone)
    {
        await _collection.InsertOneAsync(drone);
    }

    public async Task<List<DroneEntity>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }


    public async Task<DroneEntity> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.DroneId == id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveAsync(string id)
    {
        return (await _collection.DeleteOneAsync(x => x.DroneId == id))
                 .IsAcknowledged;
    }

    public async Task<UpdateResult> UpdateAsync(DroneUpdate drone)
    {
        var filter = Builders<DroneEntity>
            .Filter.Eq(d => d.DroneId, drone.DroneId);
        
        var definition = Builders<DroneEntity>.Update
            .Set(d => d.DeliveryId, drone.DeliveryId)
            .Set(d => d.Destination, drone.Destination)
            .Set(d => d.LatestStatus, drone.Status)
            .Set(d => d.CurrentLocation, drone.CurrentLocation);
        
        return await _collection.UpdateOneAsync(
            filter, definition, new UpdateOptions {IsUpsert = false});
    }


    public async Task<Dictionary<string, string>> GetAllAddresses()
    {
        return (await _collection.Find(_ => true).ToListAsync())
            .ToDictionary(
                droneModel => droneModel.DroneId, 
                droneModel => droneModel.DroneUrl
            );
    }
    
    public async Task SetDroneOffline(string droneId)
    {
        Console.WriteLine($"FleetRepository.SetDroneOffline({droneId})");
        
        var filter = Builders<DroneEntity>.Filter.Eq(d => d.DroneId, droneId);
        var definition = Builders<DroneEntity>.Update
            .Set(d => d.LatestStatus, DroneStatus.Disconnected);
        
        await _collection.UpdateOneAsync(filter, definition, new UpdateOptions 
            {IsUpsert = false});
    }
}