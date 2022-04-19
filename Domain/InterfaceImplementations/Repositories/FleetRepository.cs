using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.InterfaceDefinitions.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Domain.InterfaceImplementations.Repositories;

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

    public async Task CreateAsync(DroneRecord drone)
    {
        await _collection.InsertOneAsync(drone);
    }

    public async Task<List<DroneRecord>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<DroneRecord> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Dictionary<string, string>> GetAllAddresses() =>
        (await _collection.Find(_ => true).ToListAsync())
        .ToDictionary(droneRecord => droneRecord.Id, droneRecord => droneRecord.DroneIp);

    public async Task<bool> RemoveAsync(string id) =>
        (await _collection.DeleteOneAsync(x => x.Id == id)).IsAcknowledged;

    public async Task<bool> UpdateAsync(DroneRecord drone)
    {
        var result = await _collection.UpdateOneAsync(
            record => record.Id == drone.Id,
            GetUpdateDefinition(drone));
        return result.IsAcknowledged;
    }
        
    private static UpdateDefinition<DroneRecord> GetUpdateDefinition(DroneRecord drone)
    {
        var builder = new UpdateDefinitionBuilder<DroneRecord>();
        UpdateDefinition<DroneRecord> updateDefinition = null;
        foreach (var property in drone.GetType().GetProperties())
        {
            if (property != null) {
                if (updateDefinition == null)
                {
                    updateDefinition = builder.Set(property.Name, property.GetValue(drone));
                }
                else
                {
                    updateDefinition = updateDefinition.Set(property.Name, property.GetValue(drone));
                }
            }
        }

        return updateDefinition;
    }
}
