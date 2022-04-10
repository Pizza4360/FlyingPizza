using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.Interfaces.Gateways;

namespace Domain.Gateways
{
    public class DroneToDispatchGateway : IDispatcherGateway
    {
        // Step 4, DroneToDispatchGateway takes in initial info
        // to create a GeoLocation and then POST its first status update 
        public bool PostDroneStatus(int latitude, int longitude, string ready)
        {
            var patch
                = new DroneStatusPatch
                {
                    Location = new GeoLocation
                    {
                        Latitude = latitude
                        , Longitude = longitude
                    },
                    State = ready
                };
            var body = JsonContent.Create(patch);
            return _httpClient.PostAsync($"http://{Uri}/dispatch/send_init_status", body).Result.IsSuccessStatusCode;
        }
        
        private static HttpClient _httpClient = new HttpClient();
        private string Uri;
        public void ChangeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            _httpClient = new HttpClient(handler);
        }

        /// <summary>
        /// This method gets called when a drone updates its status.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> PatchDroneStatus(DroneStatusPatch state)
        {
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus({state})");
            var body = JsonContent.Create(state);
            var requestUri = new Uri($"http://172.18.0.0:4000/Dispatch/update_status");
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - request uri={requestUri}, body={body}"); // Debug
            var response = await _httpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - response={response}"); // Debug
            return response.IsSuccessStatusCode;
        }

        // public async Task<HttpContent> RespondToInit(string uri)
        // {
        //     var body = JsonContent.Create(
        //         $"http://{uri}/Dispatch/get_badge_number");
        //     Uri = new Uri(uri);
        //     // Console.WriteLine($"DispatcherGateway.RespondToInit - request uri={requestUri}, body={body}"); // Debug
        //     var response = await _httpClient.PostAsync(Uri, body);
        //     Console.WriteLine($"DispatcherGateway.RespondToInit - response={response}"); // Debug
        //     return response.Content;
        // }

        public int BadgeRequst()
        {
            var body = JsonContent.Create("");
            // Console.WriteLine($"DispatcherGateway.RespondToInit - request uri={requestUri}, body={body}"); // Debug
            return 1; // _httpClient.GetAsync($"http://{Uri}/dispatch/badge_request").Result.Content;
        }
    }
}
