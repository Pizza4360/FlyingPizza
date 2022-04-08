using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Domain.Implementation.Repositories
{
    public class OrdersRepository: IOrdersRepository
    {
        public Task<Order> CreateAsync(Order entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<Order> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetByIdsAsync(IEnumerable<string> ids)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Order> Update(Order entity)
        {
            throw new System.NotImplementedException();
        }
    }
}