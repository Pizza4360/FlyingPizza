using DatabaseAccess;
using Dispatch.Services;
using Domain.RepositoryDefinitions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    options.AddPolicy("CORS", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
var fleet = Environment.GetEnvironmentVariable("FLEET_COLLECTION_NAME");
var orders = Environment.GetEnvironmentVariable("ORDERS_COLLECTION_NAME");

#region repositories
var ODDSSettingsSettings = new RepositorySettings
{
    ConnectionString = connectionString,
    DatabaseName = databaseName,
    CollectionName = "Settings"
};
builder.Services.AddSingleton<ODDSSettings>(_ => new ODDSSettings(ODDSSettingsSettings));

builder.Services.AddSingleton<IOrdersRepository, OrderRepository>();

builder.Services.AddSingleton<IFleetRepository, FleetRepository>();


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

