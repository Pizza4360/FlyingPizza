using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways
{
    public class DroneToDispatchGateway : IDroneToDispatcherGateway
    {
        protected HttpClient HttpClient = new();
        private string _url;
        public string Url
        {
            // "http://172.18.0.0:4000/Dispatch"
            // "http://172.18.0.0:4000/Drone"
            set => _url = value;
            get => _url + "/Dispatch";
        }


        public void ChangeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            HttpClient = new HttpClient(handler);
        }


        

        public CompleteOrderResponse CompleteOrder(string id)
            => (CompleteOrderResponse)SendMessage(
                id,
                "CompleteOrder",
                new CompleteOrderRequest
                {
                    Time = DateTime.Now,
                    OrderId = id
                });


        public Task<BaseDto> 
            PatchDroneStatus(DroneStatusUpdateRequest state)
            => Task.FromResult(SendMessage(Url,"PatchDroneStatus", state));
        
        public BaseDto? SendMessage(string baseUri, string restCall, BaseDto dto)
        {
            var body = JsonContent.Create($"{dto.ToJsonString()}");
            var requestUri = new Uri($"{baseUri}{Url}/{restCall}");
            var result = HttpClient.PostAsync(requestUri, body).Result.Content.ReadAsStreamAsync().Result;
            return Newtonsoft.Json.JsonConvert
                .DeserializeObject<BaseDto>(result.ToString() ?? string.Empty);
        }
    }
}
