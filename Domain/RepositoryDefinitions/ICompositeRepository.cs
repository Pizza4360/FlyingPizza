using System.Collections.Generic;
using Domain.Entities;

namespace Domain.RepositoryDefinitions
{
}

namespace Domain.RepositoryDefinitions
{
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;

    public class CompositeRepository : ICompositeRepository
    {
        private readonly IMongoCollection<CompositeDocument>? _collection;
        private readonly CompositeDocument _compositeDocument;

        public CompositeRepository()
        {
            var mongoClient = new MongoClient("mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
            var mongoDatabase = mongoClient.GetDatabase("Capstone");
            _collection = mongoDatabase.GetCollection<CompositeDocument>("CompositeTest");
            _compositeDocument = _collection.Find(x => true) .FirstOrDefault();
        }
        public void AssignOrder(string orderId, string droneId)
        {
            _compositeDocument.Fleet.Find(x => x.DroneId == droneId);
        }
        public async Task CreateAsync(DroneRecord newOrder)
        {
            _compositeDocument.Fleet.Add(newOrder);
            UpdateRepo();
        }
        public async Task<List<DroneRecord>> GetDrones() => Get().Fleet;
        public async Task<List<Order>> GetOrders() => Get().Orders;
        private CompositeDocument Get() => _collection.Find(x => true) .FirstOrDefault();
        public async Task<Order> GetOrderByIdAsync(string id) 
            => (await GetOrders()).First(x => x.OrderId == id);
        public async Task<DroneRecord> GetDroneByIdAsync(string id) 
            => (await GetDrones()).First(x => x.DroneId == id);
        public async Task<bool> RemoveDroneAsync(string id)
        {
            var doc = Get();
            doc.Fleet = doc.Fleet.Where(x => x.DroneId != id).ToList();
            await UpdateRepo();
            return true;
        }
        public async Task<bool> RemoveOrderAsync(string id)
        {
            var doc = Get();
            doc.Orders = doc.Orders.Where(x => x.OrderId != id).ToList();
            await UpdateRepo();
            return true;
        }
        public async Task<bool> UpdateDroneAsync(DroneRecord entity)
        {
            var record = Get().Fleet.First(x => x.DroneId == entity.DroneId);
            record.CurrentLocation = record.CurrentLocation == null ? record.CurrentLocation : entity.CurrentLocation;
            record.DroneId = record.DroneId == null ? record.DroneId : entity.DroneId;
            record.Destination = record.Destination == null ? record.Destination : entity.Destination;
            record.DispatchUrl = record.DispatchUrl == null ? record.DispatchUrl : entity.DispatchUrl;
            record.BadgeNumber = record.BadgeNumber == null ? record.BadgeNumber : entity.BadgeNumber;
            record.DroneUrl = record.DroneUrl == null ? record.DroneUrl : entity.DispatchUrl;
            record.HomeLocation = record.HomeLocation == null ? record.HomeLocation : entity.HomeLocation;
            record.Orders = record.Orders == null ? record.Orders : entity.Orders;
            await UpdateRepo();
            return true;
        }
        public async Task<bool> UpdateOrderAsync(Order entity)
        {
            var record = Get().Orders.First(x => x.OrderId == entity.OrderId);
            record.TimeDelivered = entity.TimeDelivered;
            await UpdateRepo();
            return true;
        }
        private async Task UpdateRepo()
        {
            var update = Builders<CompositeDocument>
                        .Update
                        .Set(x => x.Orders, _compositeDocument.Orders);
            await _collection.UpdateOneAsync(x => x._id == _compositeDocument._id, update);
        }
        public async Task CreateDroneAsync(DroneRecord newDrone)
        {
            _compositeDocument.Fleet.Add(newDrone);
            await UpdateRepo();
        }
        public async Task CreateOrderAsync(Order newOrder)
        {
            _compositeDocument.Orders.Add(newOrder);
            await UpdateRepo();
        }
        public async Task<bool> Update(Order order)
        {
            foreach(
                var oldOrder 
                in _compositeDocument
                  .Orders
                  .Where(oldOrder => order.OrderId == oldOrder.OrderId)) 
            {
                oldOrder.TimeDelivered = order.TimeDelivered;
                break;
            }

            await UpdateRepo();
            return true;
        }
        public async Task<bool> RemoveOrderAsync(Order order)
        {
            _compositeDocument.Orders.Remove(order);
            await UpdateRepo();
            return true;
        }
    }
    public interface ICompositeRepository
    {
        public Task<List<DroneRecord>> GetDrones();
        public Task<List<Order>> GetOrders();
        public Task CreateDroneAsync(DroneRecord entity);
        public Task CreateOrderAsync(Order entity);
        public Task<DroneRecord> GetDroneByIdAsync(string id);
        public Task<Order> GetOrderByIdAsync(string id);
        public Task<bool> RemoveDroneAsync(string id);
        public Task<bool> RemoveOrderAsync(string id);
        public Task<bool> UpdateDroneAsync(DroneRecord entity);
        public Task<bool> UpdateOrderAsync(Order entity);
    }
}
