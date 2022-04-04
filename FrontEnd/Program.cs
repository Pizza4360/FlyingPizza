using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Radzen;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlyingPizza
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton<Services.GlobalDataSvc>(new Services.GlobalDataSvc());

            builder.Services.AddSingleton<Services.RestDbSvc>(new Services.RestDbSvc());
            
            builder.Services.AddSingleton<Services.FrontEndToDispatcherGateway>(new Services.FrontEndToDispatcherGateway());

            builder.Services.AddScoped<DialogService>();

            await builder.Build().RunAsync();
        }
    }
}
