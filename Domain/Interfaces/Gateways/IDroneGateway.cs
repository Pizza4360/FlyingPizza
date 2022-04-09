using System;
using System.Net.Http;
using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces.Gateways
{
    public interface IDroneGateway
    {
        public Task<HttpResponseMessage> StartRegistration(
            string droneIpAddress);

        public Task<bool> AssignDelivery(
            string droneId,
            string orderNumber,
            GeoLocation orderLocation);

        public Task<HttpResponseMessage> AssignToFleet(
            string droneIpAddress,
            int badgeNumber,
            string dispatcherUrl,
            GeoLocation homeLocation);
    }
}
