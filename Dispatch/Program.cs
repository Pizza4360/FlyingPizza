using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;

Console.WriteLine("hello world!!!");
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    options.AddPolicy(name: "CORS", policy =>
        policy.WithOrigins("http://localhost:83")
        .AllowAnyHeader().AllowAnyMethod()));

#region repositories
builder.Services.Configure<RepositorySettings>(builder.Configuration.GetSection("RepositorySettings"));
builder.Services.AddSingleton<ICompositeRepository>(_ => new Compository(_.GetService<IOptions<RepositorySettings>>()));
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
#endregion repositories

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Console.WriteLine(DateTime.Now);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// turn off ssl: https://stackoverflow.com/questions/43809665/enable-disable-ssl-on-asp-net-core-projects-in-development
// app.UseHttpsRedirection();
app.UseCors("CORS");
// app.UseAuthorization();
app.MapControllers();
app.Run();