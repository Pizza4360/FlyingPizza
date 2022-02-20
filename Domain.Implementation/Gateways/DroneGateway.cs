using Domain.Entities;
using Domain.Interfaces.Gateways;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Domain.Implementation.Gateways
{
    public class DroneGateway : IDroneGateway
    {
        private static HttpClient _httpClient = new HttpClient();

        public async Task<bool> AssignDeilvery(string droneIpAddress, string orderNumber, GeoLocation orderLocation)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CompleteRegistration(string droneIpAddress, string droneId, string dispatcherUrl, GeoLocation homeLocation)
        {
            throw new System.NotImplementedException();
        }

        public Drone GetDroneInfo()
        {
            throw new System.NotImplementedException();
        }
    }
}
