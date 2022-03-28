using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.Interfaces.Gateways;

namespace Domain.Implementation.Gateways
{
    public class DispatcherGateway : IDispatcherGateway
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public async Task<bool> UpdateDroneStatus(UpdateStatusDto status)
        {
            Console.WriteLine("delivering ");
            var body = JsonContent.Create(status);
            var requestUri = new Uri($"http://localhost:4000/dispatcher/assigndelivery");
            var response = await _httpClient.PostAsync(requestUri, body);
            return response.IsSuccessStatusCode;
        }
    }
}
