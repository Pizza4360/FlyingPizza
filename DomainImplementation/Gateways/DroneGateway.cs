using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Domain.Implementation.Gateways
{
    public class DroneGateway : IDroneGateway
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<bool> AssignDelivery(string droneIpAddress, string orderNumber, GeoLocation orderLocation)
        {
            var route = new List<GeoLocation> { orderLocation };

            var body = JsonContent.Create(new DeliverOrderDto
            {
                OrderId = orderNumber,
                Route = route
            });

            var requestUri = new Uri($"http://{droneIpAddress}/drone/assigndelivery");

            var response = await _httpClient.PostAsync(requestUri, body);

            return response.IsSuccessStatusCode;
        }
        
        public async Task<bool> OKToSendStatus(string droneIpAddress)
        {
            var body = JsonContent.Create(HttpStatusCode.OK);
            var requestUri = new Uri($"http://{droneIpAddress}/drone/completregistration");
            var response = await _httpClient.PostAsync(requestUri, body);
            return response.IsSuccessStatusCode;
        }

        // Todo send register command to drone with badge number & url for db
        public async Task<bool> StartRegistration(
            string droneIpAddress,
            Guid badgeNumber, // Todo ask Harrison why this is not badgeNumber
            string dispatcherUrl, 
            GeoLocation homeLocation)
        {
            var body = JsonContent.Create(badgeNumber);
            var requestUri = new Uri($"http://{droneIpAddress}/drone/initregistration");
            // response should be good
            var response = await _httpClient.PostAsync(requestUri, body);
            return response.IsSuccessStatusCode;
        }
    }
}
