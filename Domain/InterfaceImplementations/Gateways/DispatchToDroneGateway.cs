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
        public Dictionary<string, string> IdToIpMap { get; set; } = new Dictionary<string, string>();
        
        // Step 2, DispatchToDroneGateway saves the drone's url and
        // sends a POST to the drone to give it a DroneToDispatherGateway
        public async Task<InitDroneResponse> 
            InitializeRegistration(InitDroneRequest addDroneDto)
        {
            var uri = $"http://{addDroneDto.DroneIp}/Drone/InitializeRegistration";
            return (InitDroneResponse)SendMessage("InitializeRegistration", addDroneDto);
        }
        

        public AssignFleetResponse CompleteRegistration(AssignFleetRequest assignment)
        {
            return (AssignFleetResponse)SendMessage("CompleteRegistration", new AssignFleetRequest
            {
                BadgeNumber = assignment.BadgeNumber,
                DispatcherIp = assignment.DispatcherIp,
                HomeLocation = assignment.HomeLocation
            });
        }
        
        
        public async Task<AssignDeliveryResponse> AssignDelivery(AssignDeliveryRequest request)
        {
            var droneIp = IdToIpMap[request.DroneId];
            return (AssignDeliveryResponse) SendMessage(
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
    }
}