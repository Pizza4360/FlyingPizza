using Dispatch.Gateways;
using Dispatch.Repositories;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get path to file holding connection string
var path = $@"{Directory.GetCurrentDirectory()}\{Environment.GetEnvironmentVariable("ConnStrPath")}";
var connectionString = File.ReadAllText($"{path}");
Console.WriteLine(connectionString);
#region database connection
/*builder.Services.AddSingleton<IMongoClient>(_ =>
{
    return new MongoClient("mongodb://root:FlyingPizza1Here@147.182.238.228:27017");
});*/
builder.Services.AddSingleton<IMongoClient>(_ => 
    new MongoClient($"{connectionString}"));
#endregion database connection

#region repositories
builder.Services.AddScoped<IDronesRepository>(_ => new DronesRepository(
    _.GetService<IMongoClient>().GetDatabase("restheart"),
    "Fleet"));
builder.Services.AddScoped<IDronesRepository>(_ => new DronesRepository(
    _.GetService<IMongoClient>().GetDatabase("restheart"),
    "Orders"));
builder.Services.AddScoped<IDroneGateway>(_ => new DroneGateway());
#endregion repositories

/*

builder.Services.AddScoped<IDronesRepository>(builder.Services => 
    new DronesRepository(builder.Services.GetService<IMongoClient>()?.GetDatabase("restheart"),"Fleet"));

builder.Services.AddScoped<IDroneGateway>(_ => new DroneGateway());*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();