using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO.FrontEndToDispatcher;
using Domain.Entities;

namespace FrontEnd.Services
{
    public class FrontEndToDispatcherGateway
    {
        private static HttpClient _httpClient = new HttpClient();
        public async Task<GetRegistrationResultsDto> AddDrone(PostNewDroneDto droneDto)
        {
            Console.WriteLine($"FrontEndToDispatcherGateway.AddDrone({droneDto})");
            var body = JsonContent.Create(droneDto);
            var requestUri = new Uri($"http://3.84.59.41:4000/dispatcher/add_drone");
            Console.WriteLine($"- request uri={requestUri}, body={body}"); // Debug
            var response = await _httpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - response={response}"); // Debug
            return GetRegistrationResultsDto.From(response.Content);
        }
        public async Task<bool> UpdateStatus(PostAddOrderDto orderDto)
        {
            Console.WriteLine($"FrontEndToDispatcherGateway.AddDrone({orderDto})");
            var body = JsonContent.Create(orderDto);
            var requestUri = new Uri($"http://3.84.59.41:4000/dispatcher/update_status");
            Console.WriteLine($"- request uri={requestUri}, body={body}"); // Debug
            var response = await _httpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - response={response}"); // Debug
            return response.IsSuccessStatusCode;
        }
        /*
         public async Task<ResponseRegistrationDto> GetRegistrationResult(GetRegistrationResultsDto getRegDto)
        {
            Console.WriteLine($"FrontEndToDispatcherGateway.AddDrone({getRegDto})");
            var body = JsonContent.Create(getRegDto);
            var requestUri = new Uri($"http://3.84.59.41:4000/dispatcher/update_status");
            Console.WriteLine($"- request uri={requestUri}, body={body}"); // Debug
            var response = await _httpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - response={response}"); // Debug
            return ResponseRegistrationDto.From(response.Content);
        }
        */
        public async Task<bool> PostOrder(Order custOrder)
        {
            PostAddOrderDto dto = PostAddOrderDto.From(custOrder);
            Console.WriteLine($"FrontEndToDispatcherGateway.AddDrone({dto})");
            var body = JsonContent.Create(dto);
            var requestUri = new Uri($"http://3.84.59.41:4000/dispatcher/update_status");
            Console.WriteLine($"- request uri={requestUri}, body={body}"); // Debug
            var response = await _httpClient.PostAsync(requestUri, body);
            Console.WriteLine($"DispatcherGateway.UpdateDroneStatus - response={response}"); // Debug
            return response.IsSuccessStatusCode;
        }
    }
}