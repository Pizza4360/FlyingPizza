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
    private readonly IDroneRepository _drone;
    private readonly IDeliveriesRepository _deliveries;
    private readonly string _apiKey;
    private readonly string DispatchUrl;
    private readonly GeoLocation HomeLocation;

    public DatabaseAccess(OpenDroneDispatchCollectionSettings settings)
    {
        _drone = settings.GetFleetCollection();
        _deliveries = settings.GetDeliveriesCollection();
        _apiKey = settings.GetApiKey();
        HomeLocation = settings.GetHomeLocation();
        DispatchUrl = settings.GetDispatchUrl();
    }

    [HttpGet("GetFleet")]
    public async Task<List<DroneEntity>> GetFleet()
    {
        Console.WriteLine("got a request to get the fleet...");
        
        var fleet =  await _drone.GetAllAsync();
        
        Console.WriteLine("Got back" + string.Join("\n", fleet));
        
        return fleet;
    }

    [HttpGet("GetDeliveries")]
    public async Task<List<DeliveryEntity>> GetDeliveries()
    {
        Console.WriteLine("got a request to get the deliveries...");
        
        var deliveries = await _deliveries.GetAllAsync();
        
        Console.WriteLine("Got back" + string.Join("\n", deliveries));
        
        return deliveries;
    }

    [HttpPost("CancelDelivery")]
    public async Task<CancelDeliveryResponse> CancelDelivery(
        string deliveryId
      )
    {
        if (string.IsNullOrEmpty(deliveryId))
        {
            return new CancelDeliveryResponse();
        }
        
        await _deliveries.RemoveAsync(deliveryId);
        
        return new CancelDeliveryResponse();
    }

    [HttpPost("CreateDelivery")]
    public async Task<CreateDeliveryResponse> CreateDelivery(DeliveryEntity deliveryEntity)
    {
        deliveryEntity.DeliveryLocation = await LocationParser.Parse(_apiKey, 
            deliveryEntity.DeliveryAddress);
        
        Console.WriteLine($"\n\n\n\n\nadding delivery:"
                          + $" {deliveryEntity.ToJson()}\n\n\n\n\n");
        
        await _deliveries.CreateAsync(deliveryEntity);
        
        return new CreateDeliveryResponse();
    }
    
    [HttpGet("GetHomeLocation")]
    public async Task<GeoLocation> GetHomeLocation()
    {
        Console.WriteLine($"\n\n\n\nGetting HomeLocation:");
        
        return HomeLocation;
    }
  
    
    [HttpPost("AddDrone")]
    public async Task AddDrone(BaseDto @base)
    {
        var droneUrl = @base.Message;
        
        Console.WriteLine($"\n\n\n\n\n\ngot a new DRONE!!!\n@{droneUrl}");
        
        var drone = new DroneEntity
        {
            BadgeNumber = await GetNextBadgeNumber(),
            DroneId = BaseEntity<DroneEntity>.GenerateNewId(),
            DroneUrl = droneUrl,
            HomeLocation = HomeLocation,
            CurrentLocation = HomeLocation,
            Destination = HomeLocation,
            LatestStatus = DroneStatus.Unititialized,
            DeliveryId = null,
            DispatchUrl = DispatchUrl
        };
        
        Console.WriteLine("About to YEET this drone model..."
                          + $"{drone}\n\n\n\n\n");
        
        await _drone.CreateAsync(drone);
    }

    private async Task<int> GetNextBadgeNumber()
    {
        var drones = await _drone.GetAllAsync();
        
        if (!drones.Any()) { return 1; }
        
        var numbers = drones.Select(x => x.BadgeNumber);

        var enumerable = numbers.ToList();
        var maxDroneNumber = enumerable.Max();
        foreach (var i in Enumerable.Range(1, maxDroneNumber + 1))
        {
            if (!enumerable.Contains(i))
            {
                return i;
            }
        }
        return maxDroneNumber + 1;
    }

    [HttpGet("GetDrone")]
    public DroneEntity GetDrone(string id)
    {
        return _drone.GetByIdAsync(id).Result;
    }


    [HttpGet("GetDispatchUrl")]
    public string GetDispatchUrl()
    {
        return DispatchUrl;
    }

    [HttpGet("GetDeliveryById")]
    public DeliveryEntity GetDeliveryById(string id)
    {
        return _deliveries.GetByIdAsync(id).Result;
    }
}