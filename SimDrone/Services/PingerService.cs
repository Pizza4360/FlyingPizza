using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using SimDrone;

namespace Domain.Services;

public class PingerService : BackgroundService
{
    private readonly ILogger Logger;
    private readonly string DroneUrl;
    private readonly string DispatchUrl;
    private DroneRecord record;
    public const string RecordFile = "DroneRecord.json";


    public PingerService()
    {
        string text = File.ReadAllText($@"{RecordFile}");
        var record = JsonSerializer.Deserialize<DroneRecord>(text);
        DroneUrl = record.DroneUrl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(File.Exists(RecordFile))
        {
            var keepPingingDispatch = true;
            var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            while (keepPingingDispatch)
            {
                await Task.Delay(3000, stoppingToken);
                try
                {
                    var r = await _httpClient.PostAsJsonAsync($"{DroneUrl}/Revive/", record, stoppingToken);
                    Console.WriteLine($"\n\n\t\tReceived:{await r.Content.ReadAsStringAsync(stoppingToken)}\n\n\n");
                    keepPingingDispatch = await r.Content.ReadFromJsonAsync<bool>(cancellationToken: stoppingToken);
                    var cancelTask = Task.Delay(5000, stoppingToken);
                    await Task.Yield();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}