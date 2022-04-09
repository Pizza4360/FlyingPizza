using Dispatch.Gateways;
using Dispatch.Services;
using Domain.Interfaces.Gateways;


var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get path to file holding connection string
Console.WriteLine(DateTime.Now);

#region repositories

Console.WriteLine($"{builder.Configuration.GetSection("FleetDb")}");
builder.Services.Configure<OrdersDatabaseSettings>(builder.Configuration.GetSection("OrdersDb"));
builder.Services.AddSingleton<OrdersService>();
builder.Services.Configure<FleetDatabaseSettings>(builder.Configuration.GetSection("FleetDb"));
builder.Services.AddSingleton<FleetService>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddScoped<IDroneGateway>(_ => new DroneGateway());
#endregion repositories

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