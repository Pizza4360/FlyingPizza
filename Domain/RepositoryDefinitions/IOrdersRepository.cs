using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryDefinitions;

public interface IOrdersRepository : IBaseRepository<Order>
{
    public Task<IEnumerable<Order>> GetAllAsync();

    public Task<bool> UpdateOrderCompletionTime(string orderId, DateTime deliveryTime);
}