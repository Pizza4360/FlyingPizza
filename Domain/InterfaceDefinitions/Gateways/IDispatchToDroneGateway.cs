using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO.Shared;

namespace Domain.InterfaceDefinitions.Gateways
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
