using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Domain.Implementation.Gateways
{
    public class DroneGateway : IDroneGateway
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<bool> AssignDeilvery(string droneIpAddress, string orderNumber, GeoLocation orderLocation)
        {
            var route = new List<GeoLocation> { orderLocation };

            var body = JsonContent.Create(new DeliverOrderDto
            {
                OrderId = orderNumber,
                Route = route
            });

            var requestUri = new Uri($"https://{droneIpAddress}/assigndelivery");

            var response = await _httpClient.PostAsync(requestUri, body);

            return response.IsSuccessStatusCode;
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
