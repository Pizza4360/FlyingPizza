using DatabaseAccess;
using Dispatch.Services;
using Domain.Entities;
using Domain.RepositoryDefinitions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    options.AddPolicy("CORS", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));


#region repositories

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

// Todo, does this work? if not, find a way to get logging to work.
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole()
                .AddDebug()
                .AddEventSourceLogger()
                .AddFilter(_ =>
                    _.ToString().Contains("ODDS"));

// Add builder.Services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at
// https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<PingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Todo, add security
// turn off ssl
// app.UseHttpsRedirection();

app.UseCors("CORS");

app.UseAuthorization();

app.MapControllers();

app.Run();