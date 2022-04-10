using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;

namespace Domain.InterfaceDefinitions.Gateways;
public class BaseGateway : IBaseGateway<BaseDTO>
{
    public string Url { get; set; }
    private static HttpClient _httpClient = new();

    public async Task<HttpResponseMessage> SendMessage(string endpoint, BaseDTO dto)
    {
        var body = JsonContent.Create($"{dto.ToJsonString()}");
        var requestUri = new Uri($"{Url}/update_status");
        return await _httpClient.PostAsync(requestUri, body);
    }
}