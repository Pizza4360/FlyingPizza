using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways
{
    public class DispatchToDroneGateway : BaseGateway
    {
        private static HttpClient client = new HttpClient();
        public string Url;
        public Dictionary<string, string> IdToIpMap { get; set; } = new();
        
        
        public InitDroneResponse 
            InitializeRegistration(InitDroneRequest initDroneRequest)
            => (InitDroneResponse)SendMessage(
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
            client = new HttpClient(handler);
        }

        public void AddIdToIpMapping(string dtoDroneId, string dtoDroneIp)
        {
            IdToIpMap[dtoDroneId] = dtoDroneIp;
        }
    }
}