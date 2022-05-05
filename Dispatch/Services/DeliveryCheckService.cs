using System.Diagnostics;
using System.Net.Http.Headers;

namespace Dispatch.Services;

public class DeliveryCheckService : BackgroundService
{
    private readonly Stopwatch _stopwatch;
    private readonly string _endpoint;
    
    public DeliveryCheckService()
    {
        var url = Environment.GetEnvironmentVariable("DISPATCH_URL") ?? throw new Exception("The environment variable 'DISPATCH_URL' must be provided to run this program");
        _endpoint =  $"{url}/Dispatch/AssignmentCheck/";
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var httpClient = new HttpClient();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if(_stopwatch.ElapsedMilliseconds <= 3000) continue;
            _stopwatch.Reset();
            _stopwatch.Start();
            try
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var t = await httpClient.PostAsync(_endpoint, null, stoppingToken);
                await Task.Yield();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}