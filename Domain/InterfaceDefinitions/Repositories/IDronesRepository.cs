using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.InterfaceDefinitions.Repositories
{
    public interface IDronesRepository : IBaseRepository<DroneRecord>
    {
        public Task<DroneRecord> GetDroneOnOrderAsync(string orderNumber);

        public Task<IEnumerable<DroneRecord>> GetAllAvailableDronesAsync();
    }
}
