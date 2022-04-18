using Domain.InterfaceImplementations.Gateways;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add a gateway for drone to send status updates
//TODO: changed since wouldn't compile, may need to add constructor later
//builder.Services.AddScoped(_ => new DroneToDispatchGateway("Http://localhost:80"));
builder.Services.AddSingleton<DroneToDispatchGateway>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();