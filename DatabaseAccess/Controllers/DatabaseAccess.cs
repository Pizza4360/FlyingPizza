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
    private readonly GeoLocation HomeLocation;

    public DatabaseAccess(ODDSSettings settings)
    {
        _fleet = settings.GetFleetCollection();
        _orders = settings.GetOrdersCollection();
        _apiKey = settings.API_KEY;
        HomeLocation = settings.HOME_LOCATION;
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
    
    [HttpPost("GetHomeLocation")]
    public async Task<GeoLocation> GetHomeLocation(Order order)
    {
        Console.WriteLine($"\n\n\n\nGetting HomeLocation:");
        return HomeLocation;
    }
    
    

    [HttpGet("GetDrone")]
    public DroneRecord GetDrone(string id)
    {
        return _fleet.GetByIdAsync(id).Result;
    }

    [HttpGet("GetOrder")]
    public Order GetOrder(string id)
    {
        return _orders.GetByIdAsync(id).Result;
    }
}