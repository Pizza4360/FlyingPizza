using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.InterfaceImplementations.Repositories;
using Microsoft.Extensions.Options;

namespace Domain.InterfaceDefinitions.Repositories
{
    public interface IOrdersRepository : IBaseRepository<Order>
    {
        public Task<bool> PatchTimeCompleted(string id);

        public Task<Order?> GetAsync(string id);

        public Task<bool>
            CreateAsync(Order newOrder);

        public Task<Order>
            GetByIdAsync(string id);


        public Task<IEnumerable<Order>>
            GetByIdsAsync(IEnumerable<string> ids);

        public Task<bool>
            Delete(string id);

        public Task<bool>
            Update(Order order);
        
    }
}
