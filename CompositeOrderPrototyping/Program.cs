// See https://aka.ms/new-console-template for more information


using System.Text.Json;
using Dispatch;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;


Console.WriteLine("Hello, World!");

var repo = new Compository(Options.Create(new RepositorySettings
{
    ConnectionString = "mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/myFirstDatabase?retryWrites=true&w=majority",
    DatabaseName = "Capstone",
    CollectionName = "CompositeTest",
}));

var dispToSim = new DispatchToSimDroneGateway(repo);
Console.WriteLine("all the drones:\n\t" + string.Join("\n\t", repo.GetDrones().Result));
Console.WriteLine("\n\nall the orders:\n\t" + string.Join("\n\t", repo.GetOrders().Result));
Console.WriteLine("\n\nAdd a new drone in Aurora:");

var droneRecord = new DroneRecord
{
    BadgeNumber = Guid.NewGuid(),
    CurrentLocation = new GeoLocation
        {Latitude = 39.710573732539885m, Longitude = -104.81408307283085m},
    Destination = new GeoLocation
        {Latitude = 39.74386695629378m, Longitude = -105.00610500179027m},
    DispatchUrl = "http://localhost:83",
    DroneId = BaseEntity.GenerateNewId(),
    DroneUrl = "http://localhost:85",
    HomeLocation = new GeoLocation
        {Latitude = 39.74386695629378m, Longitude = -105.00610500179027m},
    State = DroneState.Returning,
    Orders = new List<Order>()
};

Console.WriteLine($"\n\nThe response from updating a document contains a copy of that entire document:\n" +
                  $"{repo.CreateDroneAsync(droneRecord).ToJson()}");

var orderDoc = $"{{\"Items\":[\"pizza\",\"artichoke\",\"pineapple\",\"olives\",\"medium\",13.42],\"CustomerName\":\"Mickey Mouse\",\"DeliveryAddress\":\"1201 5th St,Denver,CO 80204\",\"DeliveryLocation\":{droneRecord.Destination},\"HasBeenDelivered\":false}}";
Console.WriteLine($"{{\"$date\":\"{DateTime.Now.ToJson()}\"}}");
var newOrder = JsonSerializer.Deserialize<Order>(orderDoc);
newOrder.TimeOrdered = DateTime.Now;

Console.WriteLine($"\n\nAdd a new order and attach it to the new drone:\n" +
                  $"{repo.CreateOrderAsync(newOrder).Result.ToJson()}");


Console.WriteLine($"\n\nGet a drone record or an order by id. Let's test by setting the previous one to null:\n");
var orderId = newOrder.OrderId;
var droneId = droneRecord.DroneId;
droneRecord = null;
newOrder = null;

Console.WriteLine($"Before:\ndroneRecord: \"{droneRecord}\", order: {newOrder}\nWe'll wait for a few seconds to make sure the updates are finished.\n");
Thread.Sleep(15000);
droneRecord = repo.GetDroneByIdAsync(droneId).Result;
newOrder = repo.GetOrderByIdAsync(orderId).Result;
Console.WriteLine($"after:\ndroneRecord:{droneRecord.ToJson()},\nnewOrder:{newOrder}" +
                  $"Now let's assign the order to the new drone");

Console.WriteLine(repo.EnqueueOrder(new EnqueueOrderRequest{Order = newOrder}));



// dispToSim.SendMessagePost<AddDroneRequest, AddDroneResponse>(
//     droneRecord.DispatchUrl,
//     new AddDroneRequest
//     {
//         DroneId = droneRecord.DroneId,
//         BadgeNumber = droneRecord.BadgeNumber,
//         DispatchUrl = droneRecord.DispatchUrl,
//         DroneUrl = droneRecord.DroneUrl,
//         HomeLocation = droneRecord.HomeLocation
//     });



/*
    public Task<DroneRecord> RemoveDroneAsync(string id);
    public Task<Order> RemoveOrderAsync(string id);
    public Task<UpdateResult> UpdateDroneAsync(DroneRecord entity);
    public Task<UpdateResult> UpdateOrderAsync(Order entity);
 */






/*
var mongoClient = new MongoClient();
var mongoDatabase = mongoClient.GetDatabase("Capstone");
var _collection = mongoDatabase.GetCollection<CompositeDocument>("CompositeTest");
*/
/*var _id = BsonObjectId.Create("62676d0fe90cbf27acbc9cdd");
var filter = Builders<CompositeDocument>.Filter.Eq("_id", _id);*/

