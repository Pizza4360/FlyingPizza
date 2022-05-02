using System.Net.Http.Headers;
using Domain.DTO;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using SimDrone.Controllers;

namespace Domain.Services;

public class PingerService : BackgroundService
{
    private SimDroneController _controller;
    private readonly ILogger Logger;
    private string DroneUrl;
    private string DispatchUrl;
    private DroneRecord record;
    public bool KeepPingingDispatch = true;

    private HttpClient _httpClient = new();
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (KeepPingingDispatch && File.Exists(DroneRecord.File()))
        {
            try
            {
                Console.WriteLine($"\n\n'{DroneRecord.File()}' is present. Trying to revive\n\n");
                Console.WriteLine($"Reading file '{DroneRecord.File()}'");
                string text = File.ReadAllText($@"{DroneRecord.File()}");
                Console.WriteLine($"drone record was persisted as:");
                record = BsonSerializer.Deserialize<DroneRecord>(text);
                Console.WriteLine($"{text}");
                DispatchUrl = record.DispatchUrl;
                DroneUrl = record.DroneUrl;
                while (KeepPingingDispatch)
                {
                    Thread.Sleep(3000);
                    Console.WriteLine($"Sending a ping to {$"{DispatchUrl}/Dispatch/Revive"}");
                    try
                    {
                        await Task.Delay(3000, stoppingToken);
                        var r = await _httpClient.PostAsJsonAsync($"{DispatchUrl}/Dispatch/Revive", record, stoppingToken);
                        Console.WriteLine($"\n\n\t\tReceived:{await r.Content.ReadAsStringAsync(stoppingToken)}\n\n\n");
                        KeepPingingDispatch = await r.Content.ReadFromJsonAsync<bool>(cancellationToken: stoppingToken);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("No valid response from dispatch...");
                    }
                    var cancelTask = Task.Delay(5000, stoppingToken);
                    await Task.Yield();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Could not parse drone record");
            }
        }
        else
        {
            Console.WriteLine($"\n\n'{DroneRecord.File()}' not found present. Waiting for request to join \n\n");
            KeepPingingDispatch = false;
            return;
        }
        KeepPingingDispatch = false;
        var rejoinUrl = $"{DroneUrl}/SimDrone/RejoinFleet";
        Console.WriteLine($"Dispatch allowed rejoining! Sending rejoin messagage to {rejoinUrl}");
        _httpClient.PostAsJsonAsync(rejoinUrl, new ReviveRequest{Record = record});
        Console.WriteLine($"Successfully revived!");
    }
}