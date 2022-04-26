using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Entities;
using Order = Domain.Entities.Order;

namespace CompositeOrderTest;


public class OrdersDocument
{
    [BsonElement("Orders")]
    public List<Order> Orders{get;set;}
    [BsonId]
    public ObjectId _id{get;set;}
    public override string ToString()
    {
        return string.Join("\n", Orders);
    }
}

public class OrderRepository
{
    private IMongoCollection<OrdersDocument>? _collection;
    private OrdersDocument _ordersDocument;


    public OrderRepository()
    {
        var mongoClient = new MongoClient("mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
        var mongoDatabase = mongoClient.GetDatabase("Capstone");
        _collection = mongoDatabase.GetCollection<OrdersDocument>("CompositeTest");
        _ordersDocument = _collection.Find(x => true) .FirstOrDefault();
    }
    
    public async Task CreateAsync(Order newOrder)
    {
        _ordersDocument.Orders.Add(newOrder);
        UpdateRepo();
    }


    private void UpdateRepo()
    {
        /*
MongoDB.Driver.FilterDefinition<CompositeOrderPrototyping.CompositeDocument>, MongoDB.Driver.UpdateDefinition<CompositeOrderPrototyping.CompositeDocument>, MongoDB.Driver.UpdateOptions, System.Threading.CancellationToken) (in interface IMongoCollection<CompositeDocument>
         */
        var update = Builders<OrdersDocument>
                    .Update
                    .Set(x => x.Orders, _ordersDocument.Orders);
        _collection.UpdateOneAsync(x => x._id == _ordersDocument._id, update);
    }


    public Order GetByIdAsync(string id) =>
        _ordersDocument.Orders.Find(x => x.OrderId == id);

    public Task/*<bool>*/ 
        Update(Order order)
    {
        foreach(
            var oldOrder 
            in _ordersDocument
              .Orders
              .Where(oldOrder => order.OrderId == oldOrder.OrderId)) 
        {
            oldOrder.TimeDelivered = order.TimeDelivered;
            break;
        }

        UpdateRepo();
        return Task.CompletedTask;
    }

    public async Task/*<bool>*/ RemoveAsync(Order order)
    {
        _ordersDocument.Orders.Remove(order);
        UpdateRepo();
        // return Task.CompletedTask;
    }
    
    public void/* async Task<bool>*/ UpdateAsync(Order order)
    {
        foreach(
            var oldOrder 
            in _ordersDocument
              .Orders
              .Where(oldOrder => order.OrderId == oldOrder.OrderId)) 
        {
            oldOrder.TimeDelivered = order.TimeDelivered;
            break;
        }

        UpdateRepo();
        // return Task.CompletedTask;
    }
}