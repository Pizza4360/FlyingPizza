using System;
using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces.Gateways
{
    public interface IDroneGateway
    {
        public Task<bool> CompleteRegistration(string droneIpAddress, Guid badgeNumber, string 
        dispatcherUrl, GeoLocation homeLocation);

        public Task<bool> AssignDeilvery(string droneIpAddress, string orderNumber, GeoLocation orderLocation);

        public Task<bool> OKToSendStatus(string droneIpAddress);
    }
}
