using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;

namespace DatabaseAccess;

public class Program
{
    public static void Main(string[] args)
    {


        var builder = WebApplication.CreateBuilder(args);

        // OffSet services to the container.
        builder.Services.AddCors(options =>
            options.AddPolicy(name: "CORS", policy => policy.WithOrigins("*"
                ).AllowAnyHeader().AllowAnyMethod()));

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region repositories

        builder.Services.Configure<OrdersDatabaseSettings>(builder.Configuration.GetSection("OrdersDb"));
        builder.Services.AddSingleton<IOrdersRepository>(provider => new OrderRepository(provider.GetService<IOptions<OrdersDatabaseSettings>>()));

        builder.Services.Configure<FleetDatabaseSettings>(builder.Configuration.GetSection("FleetDb"));
        builder.Services.AddSingleton<IFleetRepository>(provider => new FleetRepository(provider.GetService<IOptions<FleetDatabaseSettings>>()));

        builder.Services.AddControllers()
               .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

        #endregion repositories

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if(app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.UseCors("CORS");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

// curl -X 'POST'  'http://localhost:5101/Dispatch/AddDrone'  -H 'accept: application/json'  -H 'Content-Type: application/json'  -d '{ "DroneId": "625cbcdd53108e735ee56351", "DroneId": "625cbcdd53108e735ee56351", "BadgeNumber": "ba9b96d2-fb0f-455b-b0bf-33693e171acc", "HomeLocation": { "Latitude": 39.74386695629378, "Longitude": -105.00610500179027 }, "DispatchUrl": "http://localhost:5102", "DispatchUrl": "http://localhost:5101" }'
