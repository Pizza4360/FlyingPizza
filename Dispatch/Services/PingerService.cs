using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Dispatch.Services;

public class PingerService : BackgroundService
{
    private readonly ILogger Logger;
    private readonly string _dispatchUrl; 
    public PingerService(ODDSSettings settings)
    {
        _dispatchUrl = settings.DISPATCH_URL;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var _httpClient = new HttpClient();
        var port = _dispatchUrl.Split(":").Last();
        var DispatchUrl = $"http://localhost:{port}/Dispatch";
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(3000, stoppingToken);
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var t = await _httpClient.PostAsJsonAsync($"{DispatchUrl}/AssignmentCheck/", new PingDto {S = "Hi"},
                    stoppingToken);
                // var pingTask = Pinger.SendPingAsync(IPAddress.Parse("192.168.1.100:83//Ping"), 5000);
                var cancelTask = Task.Delay(5000, stoppingToken);
                //double await so exceptions from either task will bubble up
                await Task.Yield();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}