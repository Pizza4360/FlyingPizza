using DatabaseAccess;
using Domain.Entities;
using Domain.RepositoryDefinitions;


var builder = WebApplication.CreateBuilder(args);
// Todo: Add a decent cors implementation
builder.Services.AddCors(
    options => options.AddPolicy(
        "CORS", policy => policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region repositories

var settings = new RepositorySettings
{
    ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING"),
    DatabaseName = Environment.GetEnvironmentVariable("DATABASE_NAME"),
    Collection = Environment.GetEnvironmentVariable("ODDS_SETTINGS"),
};

builder.Services.AddSingleton<IOpenDroneSystemConfigRepository
                              ,OpenDroneSystemConfigRepository>();

builder.Services.AddSingleton<IDeliveriesRepository
                              ,DeliveryRepository>();

builder.Services.AddSingleton<IDroneRepository
                              ,DroneRepository>();

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = null
                );

#endregion repositories

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CORS");

app.UseAuthorization();

app.MapControllers();

app.Run();
