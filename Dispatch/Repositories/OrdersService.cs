using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Dispatch.Repositories;
public class OrdersDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OrdersCollectionName { get; set; } = null!;
}

public class OrdersService
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

    public async Task<bool> AddOrder(Order order)
    {
        _collection.InsertOneAsync(order);
        return true;
    }
    public async Task<List<Order>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Order?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Order newOrder) =>
        await _collection.InsertOneAsync(newOrder);

    public async Task UpdateAsync(string id, Order updatedOrder) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, updatedOrder);

    public async Task RemoveAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
    // public async Task<Order> GetDroneOnOrderAsync(string orderNumber)
    // {
    //      return _collection.Find(drone => drone.OrderId == orderNumber).FirstOrDefault();
    // }
    //
    // public async Task<IEnumerable<Order>> GetAllAvailableDronesAsync()
    // {
    //     return await GetAllWhereAsync(drone => drone.State == DroneState.Ready);
    // }
}