using Dispatch.Services;
using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceImplementations.Gateways;
using Domain.InterfaceImplementations.Repositories;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IDroneGateway>(_ => new DispatchToDroneGateway());
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