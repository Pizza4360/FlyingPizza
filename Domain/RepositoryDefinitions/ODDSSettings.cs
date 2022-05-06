using DatabaseAccess;
using Domain.DTO;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public class ODDSSettings
{
    public ODDSSettings(RepositorySettings dbSettings)
    {
        var mongoClient = new MongoClient(
            dbSettings.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dbSettings.DatabaseName);

        var query = (mongoDatabase.GetCollection<ODDSSettings>("Settings").Find(x => true)).First();
        DISPATCH_URL = query.DISPATCH_URL;
        DATABASE_NAME = query.DATABASE_NAME;
        FLEET_COLLECTION_NAME = query.FLEET_COLLECTION_NAME;
        ORDERS_COLLECTION_NAME = query.ORDERS_COLLECTION_NAME;
        HOME_LOCATION = query.HOME_LOCATION;
        API_KEY = query.API_KEY;
        CONNECTION_STRING = query.CONNECTION_STRING;
        DATABASE_ACCESS_URL = query.DATABASE_ACCESS_URL;


    }
    public string _id = "settings";
    public string DISPATCH_URL { get; set;}
    public string DATABASE_ACCESS_URL { get; set; }
    public string FLEET_COLLECTION_NAME { get; set;}
    public string ORDERS_COLLECTION_NAME { get; set;}
    public GeoLocation HOME_LOCATION { get; set; }
    public string API_KEY { get; set;}
    public string CONNECTION_STRING { get; set;}
    public string DATABASE_NAME { get; set;}
}