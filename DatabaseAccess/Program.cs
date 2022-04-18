using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceImplementations.Gateways;
using Domain.InterfaceImplementations.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
    options.AddPolicy(name: "CORS", policy => policy.WithOrigins("https://localhost:44364","http://localhost:5001").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region repositories

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("OrdersDb"));
builder.Services.AddSingleton<OrderRepository>();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("FleetDb"));
builder.Services.AddSingleton<FleetRepository>();

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddScoped<IDispatchToDroneGateway>(_ => new DispatchToDroneGateway());
#endregion repositories

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("CORS");

app.UseAuthorization();

app.MapControllers();

app.Run();
