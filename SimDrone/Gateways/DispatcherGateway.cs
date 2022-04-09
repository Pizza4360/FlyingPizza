using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Interfaces.Gateways;

namespace SimDrone.Gateways
{
    public class DispatcherGateway : IDispatcherGateway
    {
        private static HttpClient _httpClient = new HttpClient();

        public void ChangeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            _httpClient = new HttpClient(handler);
        }

        public async Task<bool> PatchDroneStatus(DroneStatusPatch state)
        {
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus({state})");
            var body = JsonContent.Create(state);
            var requestUri = new Uri($"http://172.18.0.0:4000/dispatcher/update_status");
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - request uri={requestUri}, body={body}"); // Debug
            var response = await _httpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - response={response}"); // Debug
            return response.IsSuccessStatusCode;
        }
    }
}
