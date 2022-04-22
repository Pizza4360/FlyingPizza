using System.Net.Http.Headers;
using System.Net.Http.Json;
using MongoDB.Bson;

namespace Domain.GatewayDefinitions;

public class BaseGateway<T1> : IBaseGateway<T1>
{
    public HttpClient _httpClient;
    public string IpAddress{ get; set; }
    public int Port{ get; set; }
    public string IpAndPort { get => $"{IpAddress}:{Port}"; }


    public BaseGateway(int port)
    {
        _httpClient = new HttpClient();
        Port = port;
    }
    
    public async Task<TResponse?> SendMessage<TRequest, TResponse>(string url, TRequest requestDto, bool isDebug = true)
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var r = await _httpClient.PostAsJsonAsync(url, requestDto);
        Console.WriteLine($"\n\n\nSendMessage\n\t\tSent: {requestDto.ToJson()}\n\n\t\tReceived:{await r.Content.ReadAsStringAsync()}\n\n\n");
        var dto = await r.Content.ReadFromJsonAsync<TResponse>();
        return dto;
    }
}