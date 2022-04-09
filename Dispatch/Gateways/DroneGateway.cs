using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.Interfaces.Gateways;

namespace Dispatch.Gateways
{
    public class DroneGateway : IDroneGateway
    {
        private static HttpClient HttpClient = new();
        public Dictionary<string, string> IdToIpMap { get; set; } = new();

        /// <summary>
        /// This method is called to begin the delivery process.
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="orderNumber"></param>
        /// <param name="orderLocation"></param>
        /// <returns></returns>
        public async Task<bool> AssignDelivery(string droneId, string orderNumber, GeoLocation orderLocation)
        {
            Console.WriteLine($"DroneGateway.AssignDelivery({droneId}, {orderNumber}, {orderLocation})");
            var droneIp = IdToIpMap[droneId];
            var body = JsonContent.Create(new Delivery
            {
                OrderId = orderNumber,
                OrderLocation = orderLocation
            });

            var requestUri = new Uri($"http://{droneIp}/Drone/deliver");
            Console.WriteLine($"DroneGateway.AssignDelivery - request uri={requestUri}"); // Debug
            var response = await HttpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DroneGateway.AssignDelivery - response={response}"); // Debug
            return response.IsSuccessStatusCode;
        }

   

        /// <summary>
        /// This method pings a drone to begin initiation to a fleet.
        /// </summary>
        /// <param name="droneIpAddress"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> StartRegistration(string droneIpAddress)
        {
            var body = JsonContent.Create("JOIN US!!!");
            var requestUri = new Uri($"http://{droneIpAddress}/drone/initregistration");
            // response should be good
            var response = HttpClient.PostAsync(requestUri, body);
            response.Wait();
            return response;
        }

        /// <summary>
        /// After initiating registration and getting a successful response
        /// from pinging a drone, the dispatcher will call this method to
        /// assign the drone an Id and BadgeNumber. This gateway will hold
        /// the drone's Ip address in `IdToIpMap` for future requests to
        /// the drone.
        /// </summary>
        /// <param name="droneIpAddress"></param>
        /// <param name="badgeNumber"></param>
        /// <param name="dispatcherUrl"></param>
        /// <param name="homeLocation"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> AssignToFleet(
            string droneIpAddress,
            int badgeNumber,
            string dispatcherUrl,
            GeoLocation homeLocation)
        {
            var body = JsonContent.Create(new FleetAssignmentDto
            {
                BadgeNumber = badgeNumber,
                DispatcherUrl = dispatcherUrl,
                HomeLocation = homeLocation
            });
            var requestUri = new Uri($"http://{droneIpAddress}/Drone/completeregistration");
            return await HttpClient.PostAsync(requestUri, body);
        }

        public static void ChangeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            HttpClient = new HttpClient(handler);
        }
    }
}