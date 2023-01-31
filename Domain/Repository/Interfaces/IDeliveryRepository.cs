using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public interface IDeliveriesRepository 
    : IBaseRepository<DeliveryEntity, DeliveryUpdate>
{
    public Task<List<DeliveryEntity>> GetAllAsync();
    Task<UpdateResult> UpdateAsync(DeliveryUpdate update);
}