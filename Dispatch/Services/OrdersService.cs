using System.Linq.Expressions;
using System.Text.Json;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using Order = Domain.Entities.Order;

namespace Dispatch.Services;
public class OrdersDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OrdersCollectionName { get; set; } = null!;
}

public class OrdersService: IOrdersRepository
{
    private readonly IMongoCollection<Order> _collection;

    public OrdersService(IOptions<OrdersDatabaseSettings> fleetSettings)
    {
        var mongoClient = new MongoClient(
            fleetSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            fleetSettings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<Order>(
            fleetSettings.Value.OrdersCollectionName);
    }

    public async Task<Order?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<bool>
        CreateAsync(Order newOrder)
    {
        var t = _collection.InsertOneAsync(newOrder);
        t.Wait();
        if (t.IsCompletedSuccessfully)
        {
            return true;
        }
        if (t.Exception != null) Console.WriteLine(t.Exception.Message);
        return false;
    }

    public async Task<Order> 
        GetByIdAsync(string id) 
        => _collection.FindAsync(x => x.Id.Equals(id)).Result.First();


    public Task<IEnumerable<Order>> 
        GetByIdsAsync(IEnumerable<string> ids) 
        => Task.FromResult<IEnumerable<Order>>(ids.Select(id => GetByIdAsync(id).Result).ToList());

    public Task<bool> 
        Delete(string id)
    {
        
        return Task.FromResult(_collection.DeleteOne(x => x.Id.Equals(id)).IsAcknowledged);
    }

    public Task<bool> 
        Update(Order order)
    {
        var result = _collection.ReplaceOneAsync(
            new BsonDocument("_id", order.Id),
            options: new ReplaceOptions { IsUpsert = true },
            replacement: order);
        result.Wait();
        return Task.FromResult(result.IsCompletedSuccessfully);
    }

    //https://stackoverflow.com/questions/40526758/patch-rest-api-to-partial-update-mongodb-in-net
    // https://mongodb-entities.com/wiki/Code-Samples.html
    public async Task<bool> 
        PatchTimeCompleted(string id)
    {
        var doc = JsonDocument.Parse($"{{TimeDelivered:{DateTime.Now}}}").ToBsonDocument();
        var updateDefinition = new BsonDocumentUpdateDefinition<Order>(new BsonDocument("$set", doc));
        var options = new UpdateOptions();
        var token = CancellationToken.None;
        var result = await _collection.UpdateOneAsync( Builders<Order>.Filter.Eq("_id", id), updateDefinition,options, token);
        return result.IsAcknowledged;
    }

    public async Task<bool>
        PatchDroneStatus(DroneStatusPatch dto)
    {
        var doc = ToBson($"{{Id: {dto.Id}, Id:{dto.Id}, CurrentLocation:{dto.Location}}}");
        var updateDefinition = BsonDocumentUpdateDefinition(doc);
        var options = new UpdateOptions();
        var token = CancellationToken.None;
        var result = await _collection.UpdateOneAsync( 
            Filter(dto),
            updateDefinition,options, token);
        return result.IsAcknowledged;
    }
    
    private static BsonDocument 
        ToBson(string s) 
        =>  JsonDocument.Parse(s).ToBsonDocument();

    private static BsonDocumentUpdateDefinition<Order>
        BsonDocumentUpdateDefinition(BsonValue doc) 
        => new (new BsonDocument("$set", doc));

    private static FilterDefinition<Order> 
        Filter(DroneStatusPatch dto) 
        => Builders<Order>.Filter.Eq("_id", dto.Id);
}