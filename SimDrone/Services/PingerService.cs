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
using MongoDB.Bson.Serialization;
using SimDrone;

namespace Domain.Services;

public class PingerService : BackgroundService
{
    private readonly ILogger Logger;
    private string DroneUrl;
    private readonly string DispatchUrl;
    private DroneRecord record;
    public const string RecordFile = "DroneRecord.json";
    public bool KeepPingingDispatch = true;

    private HttpClient _httpClient = new();
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (KeepPingingDispatch && File.Exists(RecordFile))
        {
            try
            {
                Console.WriteLine($"\n\n'{RecordFile}' is present. Trying to revive\n\n");
                Console.WriteLine($"Reading file '{RecordFile}'");
                string text = File.ReadAllText($@"{RecordFile}");
                Console.WriteLine($"drone record was persisted as:");
                var record = BsonSerializer.Deserialize<DroneRecord>(text);
                Console.WriteLine($"{text}");
                
                DroneUrl = record.DroneUrl;
                while (KeepPingingDispatch)
                {
                    try
                    {
                        await Task.Delay(3000, stoppingToken);
                        var r = await _httpClient.PostAsJsonAsync($"{DroneUrl}/Revive/", record, stoppingToken);
                        Console.WriteLine($"\n\n\t\tReceived:{await r.Content.ReadAsStringAsync(stoppingToken)}\n\n\n");
                        KeepPingingDispatch = await r.Content.ReadFromJsonAsync<bool>(cancellationToken: stoppingToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("No valid response from dispatch...");
                    }
                    var cancelTask = Task.Delay(5000, stoppingToken);
                    await Task.Yield();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        else
        {
            Console.WriteLine($"\n\n'{RecordFile}' not found present. Waiting for request to join \n\n");
            KeepPingingDispatch = true;
        }
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}