using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IDronesRepository : IBaseRepository<DroneRecord>
    {
        public Task<DroneRecord> GetDroneOnOrderAsync(string orderNumber);

        public Task<IEnumerable<DroneRecord>> GetAllAvailableDronesAsync();
    }
}
