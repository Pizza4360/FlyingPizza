using System;
using System.Net.Http;
using System.Threading.Tasks;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace FrontEnd;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        Console.WriteLine("the dispatch url is: " + builder.Configuration.GetValue<string>("dispatchUrl"));

        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(
            _ => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

        builder.Services.AddSingleton(new GlobalDataSvc());

        builder.Services.AddScoped(_ =>
            new FrontEndToDispatchGateway(builder.Configuration.GetValue<string>("DISPATCH_URL")));
        builder.Services.AddScoped(_ => new FrontEndToDatabaseGateway(builder.Configuration.GetValue<string>("LOCAL_DB_URL")));

        builder.Services.AddSingleton(new HttpClient());

        builder.Services.AddScoped( _ =>
            new ConvertAddressToGeoLocation(builder.Configuration.GetValue<string>("API_KEY")));

        builder.Services.AddScoped<DialogService>();

        await builder.Build().RunAsync();
    }
}