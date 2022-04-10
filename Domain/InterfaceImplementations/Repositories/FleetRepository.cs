using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.InterfaceImplementations.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Dispatch.Services;

public class FleetRepository
{
    private readonly IMongoCollection<DroneRecord> _collection;
    public FleetRepository(IOptions<DatabaseSettings> fleetSettings)
    {
        var mongoClient = new MongoClient(
            fleetSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            fleetSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<DroneRecord>(
            fleetSettings.Value.CollectionName);
    }
    public async Task<List<DroneRecord>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<DroneRecord?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(DroneRecord newDroneRecord) =>
        await _collection.InsertOneAsync(newDroneRecord);

    public async Task UpdateAsync(string id, DroneRecord updatedDroneRecord) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, updatedDroneRecord);

    public async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
   
    public async Task<Dictionary<string, string>> GetAllAddresses() 
        => (await _collection.Find(_ => true)
                .ToListAsync())
            .ToDictionary(droneRecord => droneRecord.Id, droneRecord => droneRecord.IpAddress);
}
