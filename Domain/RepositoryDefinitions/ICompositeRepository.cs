using System.Collections.Generic;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using MongoDB.Bson;

namespace Domain.RepositoryDefinitions;

using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

public interface ICompositeRepository
{
    public Task<List<DroneRecord>> GetDrones();
    public Task<List<Order>> GetOrders();
    public Task<DronesOrderKey> CreateDroneAsync(DroneRecord entity);
    public Task<BsonDocument> CreateOrderAsync(Order entity);
    public Task<DroneRecord> GetDroneByIdAsync(string id);
    public Task<Order> GetOrderByIdAsync(string id);
    public Task<DeleteResult> RemoveDroneAsync(string id);
    public Task<Order> RemoveOrderAsync(string id);
    public Task<UpdateResult> UpdateDroneAsync(UpdateDroneStatusRequest request);
    public Task<UpdateResult> UpdateOrderAsync(CompleteOrderRequest request);
}