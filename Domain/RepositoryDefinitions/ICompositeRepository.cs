using System.Collections.Generic;
using Domain.Entities;

namespace Domain.RepositoryDefinitions
{
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;

    public interface ICompositeRepository
    {
        public Task<List<DroneRecord>> GetDrones();
        public Task<List<Order>> GetOrders();
        public Task CreateDroneAsync(DroneRecord entity);
        public Task CreateOrderAsync(Order entity);
        public Task<DroneRecord> GetDroneByIdAsync(string id);
        public Task<Order> GetOrderByIdAsync(string id);
        public Task<bool> RemoveDroneAsync(string id);
        public Task<Order> RemoveOrderAsync(string id);
        public Task<bool> UpdateDroneAsync(DroneRecord entity);
        public Task<bool> UpdateOrderAsync(Order entity);
    }
}
