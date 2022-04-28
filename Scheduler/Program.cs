using DatabaseAccess;
using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;
using Scheduler;
using Scheduler.Controllers;
using Scheduler = Scheduler.Controllers.Scheduler;


var builder = WebApplication.CreateBuilder(args);

// OffSet services to the container.
builder.Services.AddCors(options =>
    options.AddPolicy(name: "CORS",
        policy => policy.WithOrigins("https://localhost:44364", "http://localhost:5001", "http://localhost:81", "*")
            .AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region repositories

builder.Services.Configure<OrdersDatabaseSettings>(builder.Configuration.GetSection("OrdersDb"));
builder.Services.Configure<FleetDatabaseSettings>(builder.Configuration.GetSection("FleetDb"));


builder.Services.AddSingleton<global::Scheduler.Controllers.Scheduler>(provider =>
    new global::Scheduler.Controllers.Scheduler());

#endregion repositories

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add builder.Services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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