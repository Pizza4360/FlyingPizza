using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.Shared;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways
{
    public class DroneToDispatchGateway : BaseGateway
    {
        // Step 4, DroneToDispatchGateway takes in initial info
        // to create a GeoLocation and then POST its first status update 
        public bool PostFirstDroneStatus(int latitude, int longitude, string ready)
        {
            var patch
                = new DroneStatusUpdateRequest
                {
                    Location = new GeoLocation
                    {
                        Latitude = latitude
                        , Longitude = longitude
                    },
                    State = ready
                };
            var body = JsonContent.Create($"{patch}");
            return _httpClient.PostAsync($"http://{Url}/dispatch/SendInitialStatus", body).Result.IsSuccessStatusCode;
        }
        
        private static HttpClient _httpClient = new HttpClient();
        
        // "http://172.18.0.0:4000/Dispatch"
        // "http://172.18.0.0:4000/Drone"
        public string Url { get; set; }
        public void ChangeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            _httpClient = new HttpClient(handler);
        }

        public async Task<Task<HttpResponseMessage>> CompleteOrder(string id)
        {
            var completedOrderDto = new CompleteOrderRequest
            {
               Time = DateTime.Now,
               OrderId = id
            };
            return SendMessage("CompleteOrder", completedOrderDto);
        }
        /// <summary>
        /// This method gets called when a drone updates its status.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<Task<HttpResponseMessage>> PatchDroneStatus(DroneStatusUpdateRequest state)
        {
            return SendMessage(Url, state);
        }
    }
}
