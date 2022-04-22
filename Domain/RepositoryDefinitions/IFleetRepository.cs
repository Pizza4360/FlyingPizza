using Domain.Entities;

namespace Domain.RepositoryDefinitions;

public interface IFleetRepository : IBaseRepository<DroneRecord>
{
    public Task<List<DroneRecord>> GetAllAsync();

    public Task<Dictionary<string, string>> GetAllAddresses();
}