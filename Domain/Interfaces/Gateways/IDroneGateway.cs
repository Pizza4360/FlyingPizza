using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces.Gateways
{
    public interface IDroneGateway
    {
        public Drone GetDroneInfo();

        public Task<bool> CompleteRegistration(string droneId, string dispatcherUrl, GeoLocation homeLocation);

        public Task<bool> AssignDeilvery(string orderNumber, GeoLocation orderLocation);
    }
}
