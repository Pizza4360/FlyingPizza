using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;

namespace Domain.InterfaceDefinitions.Gateways;
public class BaseGateway : IBaseGateway<BaseDTO>
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

    public async Task<HttpResponseMessage> SendMessage(string restCall, BaseDTO dto)
    {
        var body = JsonContent.Create($"{dto.ToJsonString()}");
        var requestUri = new Uri($"{Url}/{restCall}");
        return await HttpClient.PostAsync(requestUri, body);
    }
}