﻿using System;
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

public class FleetRepository : IFleetRepository
{
    private readonly IMongoCollection<DroneRecord> _collection;
 
    public FleetRepository(IOptions<FleetDatabaseSettings> settings) //: Domain.InterfaceDefinitions.Repositories
    {
        var mongoClient = new MongoClient(
            settings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<DroneRecord>(
            settings.Value.CollectionName);
        Console.WriteLine($"this should be 'Orders'>>>{settings.Value.CollectionName}<<<");
    }


    public async Task CreateAsync(DroneRecord drone)
    {
        await _collection.InsertOneAsync(drone);
    }

    public async Task<List<DroneRecord>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    
    public async Task<DroneRecord> GetByIdAsync(string id) =>
        await _collection.Find(x => x.DroneId == id).FirstOrDefaultAsync();


    public async Task<Dictionary<string, string>> GetAllAddresses() =>
        Enumerable.ToDictionary<DroneRecord, string, string>((await _collection.Find(_ => true).ToListAsync()), droneRecord => droneRecord.DroneId, droneRecord => droneRecord.DroneUrl);

    public async Task<bool> RemoveAsync(string id) =>
        (await _collection.DeleteOneAsync(x => x.DroneId == id)).IsAcknowledged;

    public async Task<UpdateResult> UpdateAsync(DroneUpdate drone)
    {
        var filter = Builders<DroneRecord>.Filter.
            Eq(d => d.DroneId, drone.DroneId);
        var definition = Builders<DroneRecord>.Update
            .Set(d => d.OrderId, drone.OrderId)
            .Set(d => d.Destination, drone.Destination)
            .Set(d => d.State, drone.State)
            .Set(d => d.CurrentLocation, drone.CurrentLocation);
        return await _collection.UpdateOneAsync(filter, definition, new UpdateOptions {IsUpsert = false});
    }
}
