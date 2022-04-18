using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways
{
    public class DroneToDispatchGateway :IDroneToDispatcherGateway
    {
        
        protected HttpClient HttpClient = new();
                
            private string _url;
            
            public new string Url
            {
                // "http://172.18.0.0:4000/Dispatch"
                // "http://172.18.0.0:4000/Drone"
                set => _url = value;
                get => _url + "/Dispatch";
            }
            
            // to create a GeoLocation and then POST its first status update 
        public async Task<string?> PostInitialStatus(DroneStatusUpdateRequest state)
            => await SendMessage("PostInitialStatus", 
                new DroneStatusUpdateRequest 
                {
                    Id = state.Id,
                    Location = state.Location,
                    State = state.State
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
        public async Task<string?> 
            PatchDroneStatus(DroneStatusUpdateRequest state)
            => SendMessage(Url, state).Result;



        public Task<string?> SendMessage(string restCall, BaseDto dto)
        {
            var body = JsonContent.Create($"{dto.ToJsonString()}");
            var requestUri = new Uri($"{Url}/{restCall}");
            return Task.FromResult(HttpClient.PostAsync(requestUri, body)
                .Result.Content.ReadAsStreamAsync()
                .Result.ToString()!);
        }
    }
}
