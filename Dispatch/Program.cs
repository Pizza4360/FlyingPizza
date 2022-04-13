using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceImplementations.Gateways;
using Domain.InterfaceImplementations.Repositories;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

Console.WriteLine("hello world!!!");
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "*",
        _  =>
        {
            _.WithOrigins("*");
        });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add builder.Services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get path to file holding connection string
Console.WriteLine(DateTime.Now);

#region repositories

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("OrdersDb"));
builder.Services.AddSingleton<OrderRepository>();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("FleetDb"));
builder.Services.AddSingleton<FleetRepository>();

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddScoped(_ => new DispatchToDroneGateway());
#endregion repositories

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.Int32));
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// turn off ssl: https://stackoverflow.com/questions/43809665/enable-disable-ssl-on-asp-net-core-projects-in-development
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();