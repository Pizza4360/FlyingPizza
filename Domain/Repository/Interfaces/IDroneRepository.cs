using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;

namespace Domain.RepositoryDefinitions;

public interface IDroneRepository : IBaseRepository<DroneEntity, DroneUpdate>
{
    Task SetDroneOffline(string droneId);
}