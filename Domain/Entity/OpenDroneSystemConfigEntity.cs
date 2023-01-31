#nullable enable
using System.Text.Json.Serialization;
using DatabaseAccess;
using Domain.DTO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Domain.Entities;

public class OpenDroneSystemConfigRepositoryEntity 
    : BaseEntity<OpenDroneSystemConfigRepositoryEntity>, IOpenDroneSystemConfigRepository
{
    [BsonElement("ConfigId")]
    [JsonPropertyName("ConfigId")]
    public string ConfigId { get; set; }
    
    [BsonElement("DispatchUrl")]
    [JsonPropertyName("DispatchUrl")]
    public string DispatchUrl { get; set;}
    
    [BsonElement("DatabaseAccessUrl")]
    [JsonPropertyName("DatabaseAccessUrl")]
    public string DatabaseAccessUrl { get; set; }
    
    [BsonElement("CollectionName")]
    [JsonPropertyName("CollectionName")]
    public string CollectionName { get; set;}
    
    [BsonElement("HomeLocation")]
    [JsonPropertyName("HomeLocation")]
    public GeoLocation HomeLocation { get; set; }
    
    [BsonElement("ApiKey")]
    [JsonPropertyName("ApiKey")]
    public string ApiKey { get; set;}
    
    [BsonElement("ConnectionString")]
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set;}
    [BsonElement("DatabaseName")]
    [JsonPropertyName("DatabaseName")]
    public string DatabaseName { get; set;}
    
    public OpenDroneSystemConfigRepositoryEntity(RepositorySettings dbSettings)
    {
        var mongoClient = new MongoClient(dbSettings.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(dbSettings.DatabaseName);

        CollectionName = $"{OpenDroneCollectionName.Configurations}";
        
        var configCollection = mongoDatabase
            .GetCollection<OpenDroneSystemConfigRepositoryEntity>(CollectionName)
            .Find(config =>  config.ConfigId.Equals(ConfigId))
            .First();
        
        DispatchUrl = configCollection.DispatchUrl;
        DatabaseName = configCollection.DatabaseName;
        HomeLocation = configCollection.HomeLocation;
        ApiKey = configCollection.ApiKey;
        ConnectionString = configCollection.ConnectionString;
        DatabaseAccessUrl = configCollection.DatabaseAccessUrl;
    }
    
    public string GetApiKey() { return ApiKey; }
    
    public GeoLocation GetHomeLocation() { return HomeLocation; }
    
    public string GetDispatchUrl() { return DispatchUrl; }

    public override OpenDroneSystemConfigRepositoryEntity Update()
    {
        throw new System.NotImplementedException();
    }
}

public interface IOpenDroneSystemConfigRepository
{
    public string? GetApiKey();

    public GeoLocation GetHomeLocation();
    public string GetDispatchUrl();
}