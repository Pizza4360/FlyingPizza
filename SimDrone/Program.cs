Console.WriteLine(Environment.GetEnvironmentVariable("Hello"));
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    options.AddPolicy("*", policy => policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OffSet a gateway for drone to send status updates
//TODO: changed since wouldn't compile, may need to add constructor later

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CORS");
app.UseAuthorization();
app.MapControllers();
app.Run();