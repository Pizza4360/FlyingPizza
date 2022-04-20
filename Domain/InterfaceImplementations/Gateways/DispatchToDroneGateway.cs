using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways
{
    public class DispatchToDroneGateway : IDispatchToDroneGateway
    {
        protected static HttpClient HttpClient = new();
        private string _url;

        public string Url
        {
            // "http://172.18.0.0:4000/Dispatch"
            // "http://172.18.0.0:4000/Drone"
            set => _url = value;
            get => _url + "/Dispatch";
        }

        public Dictionary<string, string> IdToIpMap { get; set; } = new();


        public AssignFleetResponse AssignToFleet(AssignFleetRequest assignment)
        {
            throw new NotImplementedException();
        }

        public InitDroneResponse
            InitializeRegistration(InitDroneRequest initDroneRequest)
            => (InitDroneResponse) SendMessage(
                initDroneRequest.DroneIp,
                "InitDrone",
                initDroneRequest);

    


    public AssignFleetResponse AssignFleet(AssignFleetRequest assignment)
        {
            return (AssignFleetResponse)SendMessage(
                IdToIpMap[assignment.DroneId],
                "AssignFleet", 
                new AssignFleetRequest
            {
                BadgeNumber = assignment.BadgeNumber,
                DispatcherIp = assignment.DispatcherIp,
                HomeLocation = assignment.HomeLocation
            });
        }

    public Task<HttpResponseMessage> StartRegistration(string droneIpAddress)
    {
        throw new NotImplementedException();
    }

    public AssignDeliveryResponse AssignDelivery(AssignDeliveryRequest request)
        {
            var droneIp = IdToIpMap[request.DroneId];
            return (AssignDeliveryResponse) SendMessage(
                IdToIpMap[request.Id],
                "AssignDelivery",
                new SendDeliveryRequest
                {
                    DeliveryLocation = request.OrderLocation,
                    ID = request.OrderId
                });
        }
        

        public static void ChangeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            HttpClient = new HttpClient(handler);
        }

        public void AddIdToIpMapping(string dtoDroneId, string dtoDroneIp)
        {
            IdToIpMap[dtoDroneId] = dtoDroneIp;
        }
        public BaseDto? SendMessage(string baseUri, string restCall, BaseDto dto)
        {
            var body = JsonContent.Create($"{dto.ToJsonString()}");
            var requestUri = new Uri($"{baseUri}{Url}/{restCall}");
            var result = HttpClient.PostAsync(requestUri, body).Result.Content.ReadAsStreamAsync().Result;
            return Newtonsoft.Json.JsonConvert
                .DeserializeObject<BaseDto>(result.ToString() ?? string.Empty);
        }
    }
}