using DatabaseAccess;
using Dispatch.Services;
using Domain.RepositoryDefinitions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    options.AddPolicy("CORS", policy => policy.WithOrigins("https://localhost:44364",
        "http://localhost:5001", "http://localhost:81", "http://localhost:82",
        "http://localhost:83",
        "http://localhost:84",
        "http://localhost:85",
        "http://localhost:86",
        "http://localhost:87",
        "http://localhost:88").AllowAnyHeader().AllowAnyMethod()));

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
var fleet = Environment.GetEnvironmentVariable("FLEET_COLLECTION_NAME");
var orders = Environment.GetEnvironmentVariable("ORDERS_COLLECTION_NAME");

#region repositories
var ordersRepositorySettings = new RepositorySettings
{
    ConnectionString = connectionString,
    DatabaseName = databaseName,
    CollectionName = orders
};
builder.Services.AddSingleton<IOrdersRepository>(_ => new OrderRepository(ordersRepositorySettings));

var fleetRepositorySettings = new RepositorySettings
{
    ConnectionString = connectionString,
    DatabaseName = databaseName,
    CollectionName = fleet
};
builder.Services.AddSingleton<IFleetRepository>(provider => new FleetRepository(fleetRepositorySettings));

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

#endregion repositories

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add builder.Services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<PingerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// turn off ssl: https://stackoverflow.com/questions/43809665/enable-disable-ssl-on-asp-net-core-projects-in-development
// app.UseHttpsRedirection();

app.UseCors("CORS");

app.UseAuthorization();

app.MapControllers();

app.Run();

