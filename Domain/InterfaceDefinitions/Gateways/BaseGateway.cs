using System;
using System.Net.Http;
using System.Net.Http.Json;
using Domain.DTO;

namespace Domain.InterfaceDefinitions.Gateways
{
    public class BaseGateway : IBaseGateway<BaseDto>
    {
        /*
        public BaseGateway(string httpLocalhost)
        {
            string urlBase = httpLocalhost;
        }
        */
        protected HttpClient HttpClient = new();
        
        private string _url;
    
        public string Url
        {
            // "http://172.18.0.0:4000/Dispatch"
            // "http://172.18.0.0:4000/Drone"
            set => _url = value;
            get => _url + "/Dispatch";
        }

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
