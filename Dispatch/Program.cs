using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceDefinitions.Repositories;
using Domain.InterfaceImplementations.Repositories;
using Microsoft.Extensions.Options;

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


#region repositories

builder.Services.Configure<OrdersDatabaseSettings>(builder.Configuration.GetSection("OrdersDb"),
    System.Environment.GetEnvironmentVariable("ConnectionString"));
builder.Services.AddSingleton<IOrdersRepository>(provider =>
{
    return new OrderRepository(provider.GetService<IOptions<OrdersDatabaseSettings>>());
});

builder.Services.Configure<FleetDatabaseSettings>(builder.Configuration.GetSection("FleetDb"));
builder.Services.AddSingleton<IFleetRepository>(provider =>
{
    return new FleetRepository(provider.GetService<IOptions<FleetDatabaseSettings>>());
});

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

#endregion repositories

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add builder.Services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get path to file holding connection string
Console.WriteLine(DateTime.Now);




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