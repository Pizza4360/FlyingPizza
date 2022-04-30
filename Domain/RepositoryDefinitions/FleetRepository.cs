using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DatabaseAccess;

public class FleetRepository : IFleetRepository
{
    private readonly IMongoCollection<DroneRecord> _collection;

    public FleetRepository(IOptions<FleetDatabaseSettings> fleetSettings)
    {
        var mongoClient = new MongoClient(
            fleetSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            fleetSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<DroneRecord>(
            fleetSettings.Value.CollectionName);
    }

    public FleetRepository(RepositorySettings settings) //: Domain.InterfaceDefinitions.Repositories
    {
        var mongoClient = new MongoClient(
            settings.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.DatabaseName);

        _collection = mongoDatabase.GetCollection<DroneRecord>(
            settings.CollectionName);
        Console.WriteLine($"this should be 'Orders'>>>{settings.CollectionName}<<<");
    }


    public async Task CreateAsync(DroneRecord drone)
    {
        await _collection.InsertOneAsync(drone);
    }

    public async Task<List<DroneRecord>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }


    public async Task<DroneRecord> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.DroneId == id).FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveAsync(string id)
    {
        return (await _collection.DeleteOneAsync(x => x.DroneId == id)).IsAcknowledged;
    }

    public async Task<UpdateResult> UpdateAsync(DroneUpdate drone)
    {
        var filter = Builders<DroneRecord>.Filter.Eq(d => d.DroneId, drone.DroneId);
        var definition = Builders<DroneRecord>.Update
            .Set(d => d.OrderId, drone.OrderId)
            .Set(d => d.Destination, drone.Destination)
            .Set(d => d.State, drone.State)
            .Set(d => d.CurrentLocation, drone.CurrentLocation);
        return await _collection.UpdateOneAsync(filter, definition, new UpdateOptions {IsUpsert = false});
    }


    public async Task<Dictionary<string, string>> GetAllAddresses()
    {
        return (await _collection.Find(_ => true).ToListAsync()).ToDictionary(droneRecord => droneRecord.DroneId,
            droneRecord => droneRecord.DroneUrl);
    }
}