using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IDronesRepository : IBaseRepository<Drone>
    {
        public Task<Drone> GetDroneOnOrderAsync(string orderNumber);
    }
}
