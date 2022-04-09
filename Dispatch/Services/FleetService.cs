using System.Collections;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Dispatch.Services;
public class FleetDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string DroneRecordsCollectionName { get; set; } = null!;
}
public class FleetService
{
    private readonly IMongoCollection<DroneRecord> _collection;
    public FleetService(IOptions<FleetDatabaseSettings> fleetSettings)
    {
        var mongoClient = new MongoClient(
            fleetSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            fleetSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<DroneRecord>(
            fleetSettings.Value.DroneRecordsCollectionName);
    }
    public async Task<List<DroneRecord>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<DroneRecord?> GetAsync(string id) =>
        await _collection.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(DroneRecord newDroneRecord) =>
        await _collection.InsertOneAsync(newDroneRecord);

    public async Task UpdateAsync(string id, DroneRecord updatedDroneRecord) =>
        await _collection.ReplaceOneAsync(x => x.ID == id, updatedDroneRecord);

    public async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.ID == id);
   
    public async Task<Dictionary<string, string>> GetAllIpAddresses() 
        => (await _collection.Find(_ => true)
                .ToListAsync())
            .ToDictionary(droneRecord => droneRecord.ID, droneRecord => droneRecord.IpAddress);
}