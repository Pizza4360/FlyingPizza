using System;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
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

        builder.RootComponents.Add<App>("#app");

        var dbAccessUrl = builder.Configuration.GetValue<string>("REMOTE_DB_URL");
        var dispatchUrl = builder.Configuration.GetValue<string>("DISPATCH_URL");
        var apiKey = builder.Configuration.GetValue<string>("API_KEY");

        builder.Services.AddScoped(
            _ => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

        builder.Services.AddSingleton(new GlobalDataSvc());

        builder.Services.AddScoped(_ => new FrontEndToDispatchGateway(dispatchUrl));

        builder.Services.AddScoped(_ => new FrontEndToDatabaseGateway(dbAccessUrl));

        builder.Services.AddScoped( _ => new ConvertAddressToGeoLocation(apiKey));

        builder.Services.AddScoped<DialogService>();

        await builder.Build().RunAsync();
    }
}