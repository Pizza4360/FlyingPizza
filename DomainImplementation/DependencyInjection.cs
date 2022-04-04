using Domain.Interfaces.Gateways;
using DomainImplementation.Repositories;
using Domain.Interfaces.Repositories;
using DomainImplementation.Gateways;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DomainImplementation
{
    public static class DependencyInjection
    {
        public static void InjectDependencies(this IServiceCollection services)
        {
            #region database connection
            services.AddSingleton<IMongoClient>(services =>
            {
                return new MongoClient("mongodb+srv://mjohn314:Jh4RgVEbmmrbvQg@pdbs2021.pxbt9.mongodb.net/");
            });
            #endregion database connection

            #region repositories
            services.AddScoped<IDronesRepository>(services =>
            {
                return new DronesRepository(
                    services.GetService<IMongoClient>().GetDatabase("restheart"),
                    "Fleet");
            });
            
            services.AddScoped<IOrdersRepository>(services =>
            {
                return new OrdersRepository(
                    services.GetService<IMongoClient>().GetDatabase("restheart"),
                    "Fleet");
            });
            
            services.AddScoped<IDroneGateway>(services => new DroneGateway());
            services.AddScoped<IDispatcherGateway>(services => new DroneToDispatcherGateway());
            #endregion repositories
        }
    }
}
