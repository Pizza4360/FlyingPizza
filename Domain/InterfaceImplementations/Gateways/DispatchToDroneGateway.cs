using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.Shared;
using Domain.Entities;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways
{
    public class DispatchToDroneGateway : IDroneGateway
    {
        private HttpClient client = new HttpClient();
        public string Url;
        public Dictionary<string, string> IdToIpMap { get; set; } = new Dictionary<string, string>();
        
        // Step 2, DispatchToDroneGateway saves the drone's url and
        // sends a POST to the drone to give it a DroneToDispatherGateway
        public async Task<bool> InitializeRegistration(
        string droneUrl, string gatewayUrl, int badgeNumber)
        {
            Url = droneUrl;
            var body = JsonContent.Create(new InitDroneRequest
            {
                Url = gatewayUrl
                , BadgeNumber = badgeNumber
            });
            var requestUri = new Uri($"http://{droneUrl}/Drone/InitializeRegistration");
            // response should be good
            var ressponse = await client.PostAsync(requestUri, body);
            return ressponse.Content.Headers.ToString()
                .Contains("true");
        }
        
        // Step 6, use the incoming DroneRecord to create a SimDrone.Drone
        // object, and use DispatchToDroneGateway to POST it to the
        // DroneController
        public async Task<HttpResponseMessage> CompleteRegistration(DroneRecord record)
        {
            var body = JsonContent.Create(record);
            var requestUri = new Uri($"http://{Url}/drone/StartDrone");
            // response should be good
            return await client.PostAsync(requestUri, body);
        }

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
            var body = JsonContent.Create(new AssignDeliveryRequest
            {
                OrderId = orderNumber,
                OrderLocation = orderLocation
            });

            var requestUri = new Uri($"http://{droneIp}/Drone/deliver");
            Console.WriteLine($"DroneGateway.AssignDelivery - request uri={requestUri}"); // Debug
            var response = await client.PostAsync(requestUri, body);
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
            var response = client.PostAsync(requestUri, body);
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
            var body = JsonContent.Create(new AssignFleetRequest
            {
                BadgeNumber = badgeNumber,
                DispatcherUrl = dispatcherUrl,
                HomeLocation = homeLocation
            });
            var requestUri = new Uri($"http://{droneIpAddress}/Drone/CompleteRegistration");
            return await client.PostAsync(requestUri, body);
        }


        // public static void ChangeHandler(HttpMessageHandler handler)
        // {
        //     // Added for mocking reasons, no way around it
        //     HttpClient = new HttpClient(handler);
        // }
        public JsonContent Serialize(BaseDTO dto)
        {
            throw new NotImplementedException();
        }

        public BaseDTO SendMessage(HttpResponseMessage response)
        {
            throw new NotImplementedException();
        }
    }
}