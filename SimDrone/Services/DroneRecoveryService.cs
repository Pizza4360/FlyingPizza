using System.Diagnostics;
using Domain.DTO;
using Domain.Entities;
using MongoDB.Bson.Serialization;

namespace SimDrone.Services;

public class DroneRecoveryService : BackgroundService
{
    private string _droneUrl;
    private string _dispatchUrl;
    private DroneRecord _record;
    private bool _keepPingingDispatch = true;
    private HttpClient _httpClient = new();
    private Stopwatch _stopwatch;

    public DroneRecoveryService()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_keepPingingDispatch && File.Exists(DroneRecord.File()))
        {
            try
            {
                Console.WriteLine($"\n\n'{DroneRecord.File()}' is present. Trying to recover\n\n");
                Console.WriteLine($"Reading file '{DroneRecord.File()}'");
                var text = await File.ReadAllTextAsync($@"{DroneRecord.File()}", stoppingToken);
                Console.WriteLine($"drone record was persisted as:");
                _record = BsonSerializer.Deserialize<DroneRecord>(text);
                Console.WriteLine($"{text}");
                _dispatchUrl = _record.DispatchUrl;
                _droneUrl = _record.DroneUrl;
                while (_keepPingingDispatch)
                {
                    if(_stopwatch.ElapsedMilliseconds <= 3000) continue;
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    Console.WriteLine($"Sending a ping to {_dispatchUrl}/Dispatch/Recover");
                    try
                    {
                        // await Task.Delay(3000, stoppingToken);
                        var r = await _httpClient.PostAsJsonAsync($"{_dispatchUrl}/Dispatch/Recover", _record, stoppingToken);
                        Console.WriteLine($"\n\n\t\tReceived:{await r.Content.ReadAsStringAsync(stoppingToken)}\n\n\n");
                        _keepPingingDispatch = await r.Content.ReadFromJsonAsync<bool>(cancellationToken: stoppingToken);
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
            _keepPingingDispatch = true;
            return;
        }
        _keepPingingDispatch = false;
        var rejoinUrl = $"{_droneUrl}/SimDrone/RejoinFleet";
        Console.WriteLine($"Dispatch allowed rejoining! Sending rejoin message to {rejoinUrl}.");
        await _httpClient.PostAsJsonAsync(rejoinUrl, new DroneRecoveryRequest{Record = _record}, cancellationToken: stoppingToken);
        Console.WriteLine("Successfully recovered!");
    }
}