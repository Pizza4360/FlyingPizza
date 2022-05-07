using DatabaseAccess.Services;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseAccess.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseAccess : ControllerBase
{
    private readonly IFleetRepository _fleet;
    private readonly IOrdersRepository _orders;
    private readonly string _apiKey;
    private readonly string DispatchUrl;
    private readonly GeoLocation HomeLocation;

    public DatabaseAccess(ODDSSettings settings)
    {
        _fleet = settings.GetFleetCollection();
        _orders = settings.GetOrdersCollection();
        _apiKey = settings.API_KEY;
        HomeLocation = settings.HOME_LOCATION;
        DispatchUrl = settings.DISPATCH_URL;
    }

    [HttpGet("GetFleet")]
    public async Task<List<DroneRecord>> GetFleet()
    {
        Console.WriteLine("got a request to get the fleet...");
        var fleet = await _fleet.GetAllAsync();
        Console.WriteLine("Got back" + string.Join("\n", fleet));
        return fleet;
    }

    [HttpPost("CreateOrder")]
    public async Task<CreateOrderResponse> CreateOrder(Order order)
    {
        order.DeliveryLocation = await LocationParser.Parse(_apiKey, order.DeliveryAddress);
        Console.WriteLine($"\n\n\n\n\nadding order: {order.ToJson()}\n\n\n\n\n");
        await _orders.CreateAsync(order);
        return new CreateOrderResponse();
    }
    
    [HttpGet("GetHomeLocation")]
    public async Task<GeoLocation> GetHomeLocation()
    {
        Console.WriteLine($"\n\n\n\nGetting HomeLocation:");
        return HomeLocation;
    }
  
    
    [HttpPost("AddDrone")]
    public async Task AddDrone(PingDto ping)
    {
        var droneUrl = ping.S;
        Console.WriteLine($"\n\n\n\n\n\ngot a new DRONE!!!\n@{droneUrl}");
        var drone = new DroneRecord
        {
            DroneId = BaseEntity.GenerateNewId(),
            DroneUrl = droneUrl,
            HomeLocation = HomeLocation,
            CurrentLocation = HomeLocation,
            Destination = HomeLocation,
            State = DroneState.Unititialized,
            OrderId = null,
            DispatchUrl = ""
        };
        Console.WriteLine($"About to YEET this drone record...{drone}\n\n\n\n\n");
        await _fleet.CreateAsync(drone);
    }
    
    [HttpGet("GetDrone")]
    public DroneRecord GetDrone(string id)
    {
        return _fleet.GetByIdAsync(id).Result;
    }


    [HttpGet("GetDispatchUrl")]
    public string GetDispatchUrl()
    {
        return DispatchUrl;
    }

    [HttpGet("GetOrder")]
    public Order GetOrder(string id)
    {
        return _orders.GetByIdAsync(id).Result;
    }
}