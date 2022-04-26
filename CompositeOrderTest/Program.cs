// See https://aka.ms/new-console-template for more information


using Domain.DTO;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using MongoDB.Bson;
using MongoDB.Driver;


Console.WriteLine("Hello, World!");

var mongoClient = new MongoClient("mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
var mongoDatabase = mongoClient.GetDatabase("Capstone");
var _collection = mongoDatabase.GetCollection<CompositeDocument>("CompositeTest");


var repo = new CompositeRepository();
Console.WriteLine(repo);

var _id = BsonObjectId.Create("62676d0fe90cbf27acbc9cdd");
var filter = Builders<CompositeDocument>.Filter.Eq("_id", _id);
/*
    public Task<List<DroneRecord>> GetDrones();
    public Task<List<Order>> GetOrders();
    public Task<Task<CompositeDocument>> CreateDroneAsync(DroneRecord entity);
    public Task<Task<CompositeDocument>> CreateOrderAsync(Order entity);
    public Task<DroneRecord> GetDroneByIdAsync(string id);
    public Task<Order> GetOrderByIdAsync(string id);
    public Task<DroneRecord> RemoveDroneAsync(string id);
    public Task<Order> RemoveOrderAsync(string id);
    public Task<UpdateResult> UpdateDroneAsync(DroneRecord entity);
    public Task<UpdateResult> UpdateOrderAsync(Order entity);
 */

// var order = new Order
// {
//     CustomerName = "SMOOTH",
//     DeliveryAddress = "blah",
//     TimeOrdered = DateTime.Now
// };

var i = 0;
var orders = new List<Order>();
var homeLocation = new GeoLocation{Latitude = 0.1m,Longitude = 0.2m};

var update = Builders<CompositeDocument>
    .Update.Set($"Fleet.{i}.Orders", orders)
    .Set($"Fleet.{i}.State", DroneState.Ready)
    .Set($"Fleet.{i}.CurrentLocation", homeLocation);
var result = _collection.UpdateOne(filter, update, new UpdateOptions {IsUpsert = true});

Console.WriteLine(result);
// .Set("Orders", _compositeDocument.Orders); 
