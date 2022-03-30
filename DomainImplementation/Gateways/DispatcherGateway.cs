using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.Interfaces.Gateways;

namespace Domain.Implementation.Gateways
{
    public class DispatcherGateway : IDispatcherGateway
    {
        private static HttpClient _httpClient = new HttpClient();
        public async Task<bool> PutDroneState(UpdateStatusDto status)
        {
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus({status})");
            var body = JsonContent.Create(status);
            var requestUri = new Uri($"http://172.18.0.0:4000/dispatcher/update_status");
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - request uri={requestUri}, body={body}"); // Debug
            var response = await _httpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - response={response}"); // Debug
            return response.IsSuccessStatusCode;
        }
        
        public void changeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            _httpClient = new HttpClient(handler);
        }
        
    }
}
