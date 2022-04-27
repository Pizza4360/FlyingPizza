using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;

Console.WriteLine("hello world!!!");
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    options.AddPolicy(name: "CORS", policy => policy.WithOrigins("https://localhost:44364", 
    "http://localhost:5001", "http://localhost:81", "http://localhost:82",
    "http://localhost:83",
    "http://localhost:84",
    "http://localhost:85",
    "http://localhost:86",
    "http://localhost:87",
    "http://localhost:88").AllowAnyHeader().AllowAnyMethod()));

#region repositories

builder.Services.Configure<RepositorySettings>(builder.Configuration.GetSection("RepositorySettings"));
builder.Services.AddSingleton<ICompositeRepository>(provider 
    => new Compository(provider.GetService<IOptions<RepositorySettings>>()));
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

app.UseCors("CORS");

app.UseAuthorization();

app.MapControllers();

app.Run();