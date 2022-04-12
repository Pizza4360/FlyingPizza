using System;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.InterfaceImplementations.Gateways;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace FrontEnd
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton<Services.GlobalDataSvc>(new Services.GlobalDataSvc());
            builder.Services.AddSingleton<FrontEndToDispatchGateway>(new FrontEndToDispatchGateway("http://localhost:82"));

            builder.Services.AddSingleton<Services.RestDbSvc>(new Services.RestDbSvc());

            builder.Services.AddScoped<DialogService>();

            await builder.Build().RunAsync();
        }
    }
}
