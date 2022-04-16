using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceImplementations.Gateways;

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