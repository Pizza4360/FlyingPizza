using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.InterfaceDefinitions.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.InterfaceImplementations.Repositories;

public class FleetRepository: IFleetRepository
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

    public Task<DroneRecord> GetByIdAsync(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<DroneRecord>> GetByIdsAsync(IEnumerable<string> ids)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> Delete(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> Update(DroneRecord entity)
    {
        throw new System.NotImplementedException();
    }

    public async Task UpdateAsync(string id, DroneRecord updatedDroneRecord) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, updatedDroneRecord);

    public async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
   
    public async Task<Dictionary<string, string>> GetAllAddresses() 
        => (await _collection.Find(_ => true)
                .ToListAsync())
            .ToDictionary(droneRecord => droneRecord.Id, droneRecord => droneRecord.IpAddress);

    public async Task<UpdateResult> PatchDroneStatus(DroneStatusUpdateRequest dto)=>
         await _collection.UpdateOneAsync( 
            Filter(dto)
            , BsonDocumentUpdateDefinition(ToBson(dto))
            ,new UpdateOptions()
            , CancellationToken.None);
        
    private static BsonDocumentUpdateDefinition<DroneRecord>
        BsonDocumentUpdateDefinition(BsonValue doc) 
        => new (new BsonDocument("$set", doc));
    
    private static BsonDocument 
        ToBson(BaseDTO dto) 
        =>  JsonDocument.Parse($"{dto}").ToBsonDocument();

    private static FilterDefinition<DroneRecord> 
        Filter(DroneStatusUpdateRequest dto)
    => Builders<DroneRecord>.Filter.Eq("_id", dto.Id);

    Task<bool> IBaseRepository<DroneRecord>.CreateAsync(DroneRecord entity)
    {
        throw new System.NotImplementedException();
    }
}
