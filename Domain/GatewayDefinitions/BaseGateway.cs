﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MongoDB.Bson;


namespace Domain.GatewayDefinitions;

public class BaseGateway<T1> : IBaseGateway<T1>
{
    public HttpClient _httpClient;

    public BaseGateway()
    {
        _httpClient = new HttpClient();
    }

    public async Task<TResponse> SendMessageGet<TResponse>(string url)
    {
        return await HttpMethods.Get<TResponse>(url);
    }
    
    public async Task<TResponse?> SendMessagePost<TRequest, TResponse>(string url, TRequest requestDto, bool isDebug = true)
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Console.WriteLine($"\n\n\nSendMessage\n\t\tIP Url: {url}\n\t\tSent: {requestDto.ToJson()}");
        var r = _httpClient.PostAsJsonAsync(url, requestDto);
        r.Wait();
        Console.WriteLine($"\n\n\t\tReceived:{await r.Result.Content.ReadAsStringAsync()}\n\n\n");
        var dto = await r.Result.Content.ReadFromJsonAsync<TResponse>();
        return dto;
    }
}
