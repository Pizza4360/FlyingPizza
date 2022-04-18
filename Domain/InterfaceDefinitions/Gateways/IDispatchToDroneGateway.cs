using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;

namespace Domain.InterfaceDefinitions.Gateways
{
    public interface IDispatchToDroneGateway : IBaseGateway<BaseDto>
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

        Task<bool> InitializeRegistration(string droneIp, string httpsFlyingpizzaCom, Guid testBadgeNumber);

        public Task<HttpResponseMessage> CompleteRegistration(DroneRecord record);

        Dictionary<string, string> IdToIpMap { get; set; }
    }
}
