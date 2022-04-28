using System;
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
    public Task<Tuple<DroneRecord, Assignment>> CreateDroneAsync(DroneRecord entity);
    public Task<Order> EnqueueOrder(Order entity);
    public Task<DroneRecord> GetDroneByIdAsync(string id);
    public Task<Order> GetOrderByIdAsync(string id);
    public Task<DeleteResult> RemoveDroneAsync(string id);
    public Task<DeleteResult> RemoveOrderAsync(string id);
    public Task<UpdateResult> UpdateDroneAsync(UpdateDroneStatusRequest request);
    public Task<UpdateResult> UpdateOrderAsync(CompleteOrderRequest request);
    public Task<IEnumerable<AssignDeliveryRequest>> GenerateDeliveryRequests();

    public Task UpdateAssignmentAsync(string droneId, bool shouldBeNotified);
}