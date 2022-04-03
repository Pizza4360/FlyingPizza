using Domain.Entities;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace DomainImplementation.Repositories
{
    public class DronesRepository : MongoRepository<Drone>, IDronesRepository
    {
        public DronesRepository(IMongoDatabase database, string collectionName)
            : base(database, collectionName)
        { }
        
        public async Task<Drone> GetDroneOnOrderAsync(string orderNumber)
        {
            return (await GetAllWhereAsync(drone => drone.OrderId == orderNumber)).FirstOrDefault();
        }

        public async Task<IEnumerable<Drone>> GetAllAvailableDronesAsync()
        {
            return await GetAllWhereAsync(drone => drone.Status == Constants.DroneStatus.READY);
        }
    }
}
