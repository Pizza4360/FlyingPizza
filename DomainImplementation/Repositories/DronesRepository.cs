using Domain.Entities;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainImplementation.Repositories
{
    public class DronesRepository : MongoRepository<DroneRecord>, IDronesRepository
    {
        public DronesRepository(IMongoDatabase database, string collectionName)
            : base(database, collectionName)
        { }
        
        public async Task<DroneRecord> GetDroneOnOrderAsync(string orderNumber)
        {
            return (await GetAllWhereAsync(drone => drone.OrderId == orderNumber)).FirstOrDefault();
        }

        public async Task<IEnumerable<DroneRecord>> GetAllAvailableDronesAsync()
        {
            return await GetAllWhereAsync(drone => drone.State == DroneState.Ready);
        }
    }
}
