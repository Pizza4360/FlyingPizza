using Domain.DTO.DroneDispatchCommunication;
using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Abstractions;

namespace Tests.Controllers.EndToEnd;
[TestClass]
public class FleetRepositoryServiceTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public FleetRepositoryServiceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /*public FleetRepositoryServiceTest()
    {
        [TestMethod]
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

// Get path to file holding connection string
        Console.WriteLine(DateTime.Now);

        #region repositories

        Console.WriteLine($"{builder.Configuration.GetSection("FleetDb")}");
        builder.Services.Configure<OrdersDatabaseSettings>(builder.Configuration.GetSection("OrdersDb"));
        builder.Services.AddSingleton<OrdersService>();
        builder.Services.Configure<FleetDatabaseSettings>(builder.Configuration.GetSection("FleetDb"));
        builder.Services.AddSingleton<FleetService>();
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
    }*/
    
    [Fact]
    public void TestAddDroneHandshake()
    {
        var dotNetObj = new InitGatewayPost
        {
            Url = "localhost:7100"
            , BadgeNumber = 1
        };
        _testOutputHelper.WriteLine($"{dotNetObj}"); 
        //     dotNetObj.ToBsonDocument()
        // BsonTypeMapper.MapToDotNetValue());
        // JsonConvert.SerializeObject();JsonConvert.SerializeObject(dotNetObj);
        // var expected = @"{""BadgeNumber"":1,""Url"":""localhost:7100""}";
        // Assert.Equal(expected, body.ToString());
        
        
        /*
        var dispatch2Drone = new DispatchToDroneGateway()
        {
            Url = "localhost:"
        };
        var controller = new SimDroneController();
        var result = dispatch2Drone.InitRegistration(
            "localhost:7100"
            , "localhost:7101"
            , 1).Result;
            */

    }
}
