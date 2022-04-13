using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;

namespace Domain.InterfaceDefinitions.Gateways
{
    public interface IDispatchToDroneGateway : IBaseGateway<BaseDTO>
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

        Task<bool> InitializeRegistration(string droneIp, string httpsFlyingpizzaCom, int testBadgeNumber);

        public Task<HttpResponseMessage> CompleteRegistration(DroneRecord record);
        
    }
}
