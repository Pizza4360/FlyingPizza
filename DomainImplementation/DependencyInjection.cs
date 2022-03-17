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
                return new MongoClient("mongodb://localhost:27018/?readPreference=primary&appname=MongoDB%20Compass&directConnection=true&ssl=false");
            });
            #endregion database connection

            #region repositories
            services.AddScoped<IDronesRepository>(services =>
            {
                return new DronesRepository(
                    services.GetService<IMongoClient>().GetDatabase("restheart"),
                    "Fleet");
            });
            services.AddScoped<IDispatcherGateway>(services => new DispatcherGateway());
            #endregion repositories
        }
    }
}
