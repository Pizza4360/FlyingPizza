using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Domain.Implementation.Gateways
{
    public class DroneGateway : IDroneGateway
    {
        private static HttpClient HttpClient = new HttpClient();

        public async Task<bool> AssignDelivery(string droneIpAddress, string orderNumber, GeoLocation orderLocation)
        {
            Console.WriteLine($"DroneGateway.AssignDelivery({droneIpAddress}, {orderNumber}, {orderLocation})");
            var body = JsonContent.Create(new DeliverOrderDto
            {
                OrderId = orderNumber,
                OrderLocation = orderLocation
            });

            var requestUri = new Uri($"http://{droneIpAddress}/drone/assigndelivery");
            Console.WriteLine($"DroneGateway.AssignDelivery - request uri={requestUri}"); // Debug
            var response = await HttpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DroneGateway.AssignDelivery - response={response}"); // Debug
            return response.IsSuccessStatusCode;
        }
        
        public async Task<bool> OKToSendStatus(string droneIpAddress)
        {
            var body = JsonContent.Create(HttpStatusCode.OK);
            var requestUri = new Uri($"http://{droneIpAddress}/drone/completeregistration");
            var response = await HttpClient.PostAsync(requestUri, body);
            return response.IsSuccessStatusCode;
        }

        // Todo send register command to drone with badge number & url for db
        public async Task<bool> StartRegistration(
            string droneIpAddress,
            Guid badgeNumber,
            string dispatcherUrl, 
            GeoLocation homeLocation)
        {
            var body = JsonContent.Create(badgeNumber);
            var requestUri = new Uri($"http://{droneIpAddress}/drone/initregistration");
            // response should be good
            var response = await HttpClient.PostAsync(requestUri, body);
            return response.IsSuccessStatusCode;
        }

        public void changeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            HttpClient = new HttpClient(handler);
        }
    }
}
