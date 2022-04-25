using System;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
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

        builder.Services.AddScoped(
            _ => new HttpClient
            {
                BaseAddress = new Uri(builder
                                     .HostEnvironment
                                     .BaseAddress)
            });

        builder.Services.AddSingleton(new GlobalDataSvc());

        builder.Services.AddSingleton(new FrontEndToDispatchGateway());
        builder.Services.AddSingleton(new FrontEndToDatabaseGateway());

        builder.Services.AddSingleton(new HttpClient());

        builder.Services.AddSingleton(new ConvertAddressToGeoLocation());
       /* { 
           ApiKey = builder.Configuration.GetValue<string>("MapsApiKey")
        });*/

        builder.Services.AddScoped<DialogService>();

        await builder.Build().RunAsync();



    }
}
