using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.RepositoryDefinitions;

public interface IFleetRepository : IBaseRepository<DroneRecord>
{
    public Task<List<DroneRecord>> GetAllAsync();

    /// <summary>
    /// Updates a drone's status and location
    /// </summary>
    /// <param name="drone"></param>
    /// <returns></returns>
    public Task<bool> UpdateStatusAndLocationAsync(DroneRecord drone);
}