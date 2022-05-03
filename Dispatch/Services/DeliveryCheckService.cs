using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dispatch.Services;

public class DeliveryCheckService : BackgroundService
{
    private readonly ILogger Logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var _httpClient = new HttpClient();
        var DispatchUrl = "http://localhost:83" + "/Dispatch";
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