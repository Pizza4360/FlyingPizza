using System;
using System.Threading.Tasks;
using DatabaseAccess;
using Domain.DTO;
using Domain.Entities;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;
/**
 * Todo,
 * make IBaseRepository to extend ISettingsRepository, move the field
 * _collection to that class as a protected members, then make this class extend
 * IBaseRepository.
 */
public class OpenDroneSystemConfigRepository : IOpenDroneSystemConfigRepository
{
    private readonly IMongoCollection<OpenDroneSystemConfigRepositoryEntity> _collection;

    public OpenDroneSystemConfigRepository(RepositorySettings settings)
    {
        var mongoClient = new MongoClient(
            settings.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.DatabaseName);

        _collection = mongoDatabase.GetCollection<OpenDroneSystemConfigRepositoryEntity>(
            settings.Collection);
    }

    public string GetApiKey() { throw new NotImplementedException(); }

    GeoLocation IOpenDroneSystemConfigRepository.GetHomeLocation() { throw new NotImplementedException(); }

    public string GetDispatchUrl() { throw new NotImplementedException(); }

    public async Task<GeoLocation> GetHomeLocation()
    {
        return (await _collection.FindAsync(x => true)).First().HomeLocation;
    }
}