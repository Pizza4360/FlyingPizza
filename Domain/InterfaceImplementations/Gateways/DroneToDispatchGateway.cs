using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways
{
    public class DroneToDispatchGateway : BaseGateway
    {
        // Step 4, DroneToDispatchGateway takes in initial info
        // to create a GeoLocation and then POST its first status update 
        public async Task<string?> PostInitialStatus(
        int latitude,
        int longitude,
        string ready) 
            => await SendMessage("PostInitialStatus", 
                new DroneStatusUpdateRequest 
                {
                    Location = new GeoLocation
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    },
                    State = ready
                });

        public void ChangeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            HttpClient = new HttpClient(handler);
        }

        public async Task<string?> CompleteOrder(string id)
            => await SendMessage("CompleteOrder",
                new CompleteOrderRequest
                {
                    Time = DateTime.Now,
                    OrderId = id
                });

        /// <summary>
        /// This method gets called when a drone updates its status.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<Task<Task<string?>>> 
            PatchDroneStatus(DroneStatusUpdateRequest state)
            => Task.FromResult(SendMessage(Url, state));

        public DroneToDispatchGateway(string httpLocalhost)
        {
        }
    }
}
