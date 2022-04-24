using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.InterfaceDefinitions.Repositories
{
    public interface IFleetRepository : IBaseRepository<DroneRecord>
    {
        public Task<List<DroneRecord>> GetAllAsync();

        public Task<Dictionary<string, string>> GetAllAddresses();
    }
}
