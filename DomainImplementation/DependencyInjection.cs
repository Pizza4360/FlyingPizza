using Domain.Implementation.Gateways;
using Domain.Interfaces.Gateways;
using DomainImplementation.Repositories;
using Domain.Interfaces.Repositories;
using DomainImplementation.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Domain.Implementation
{
    public static class DependencyInjection
    {
        public static void InjectDependencies(this IServiceCollection services)
        {
            #region database connection
            services.AddSingleton<IMongoClient>(services =>
            {
                return new MongoClient("mongodb://root:FlyingPizza1Here@147.182.238.228:27017");
            });
            #endregion database connection

            #region repositories
            services.AddScoped<IDronesRepository>(services =>
            {
                return new DronesRepository(
                    services.GetService<IMongoClient>().GetDatabase("restheart"),
                    "Fleet");
            });
            services.AddScoped<IDroneGateway>(services => new DroneGateway());
            services.AddScoped<IDispatcherGateway>(services => new DispatcherGateway());
            #endregion repositories
        }
    }
}
