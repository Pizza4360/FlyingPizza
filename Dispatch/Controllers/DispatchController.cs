using Dispatch.Gateways;
using Dispatch.Services;
using Domain;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Order = Domain.Entities.Order;

namespace Dispatch.Controllers;

[ApiController]
[Route("[controller]")]
public class DispatchController : ControllerBase
{
    private readonly FleetService _droneRepo;
    private readonly OrdersService _orderRepo;
    private readonly DroneGateway _droneGateway;
    private readonly ILogger<DispatchController> _logger;
    private readonly GeoLocation HomeLocation;
    private readonly Queue<Delivery> _unfilledOrders;

    /// <summary>
    /// Call this method for debugging and testing if the dispatcher
    /// is alive.
    /// </summary>
    /// <returns></returns>
    [HttpPost("ping")]
    public string Ping() => "I'm alive!";

    public DispatchController(FleetService droneRepo, OrdersService orderRepo, GeoLocation homeLocation, DroneGateway droneGateway)
    {
        _droneRepo = droneRepo;
        _orderRepo = orderRepo;
        _droneGateway = droneGateway;
        _droneGateway.IdToIpMap = _droneRepo.GetAllIpAddresses().Result;
        HomeLocation = homeLocation;
    }
    

    /// <summary>
    /// A drone will call this method when their assigned order has been completed.
    /// The delivery time of the order will be patched.
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    [HttpPatch("complete_order")]
    public async Task<bool> 
        PatchDeliveryTime(Order order) 
        => _orderRepo.PatchTimeCompleted(order.Id).Result;

    /// <summary>
     /// This method is invoked from the front to add a new drone to the fleet.
     /// </summary>
     /// <param name="dto"></param>
     /// <returns>`true` only if the handshake is completed and the drone is initialized.</returns>
     [HttpPost("register")]
     public async Task<bool> StartFleetRegistration(GetInitDroneResponse dto)
     {
         
         Console.WriteLine("Attempting to initialize communication with drone...");
         var canBeInitialized = _droneGateway.StartRegistration($"{dto.IpAddress}/Drone/init_registration");
         if (!canBeInitialized.IsCompletedSuccessfully)
         {
             Console.WriteLine("The drone could not be initialized...");
             return false;
         }
         // Todo make a new guid and make sure it is different from all other drones
         var newDrone = new DroneRecord
         {
             BadgeNumber = dto.BadgeNumber,
             IpAddress = dto.IpAddress,
             HomeLocation = HomeLocation,
             DispatcherUrl = "//172.18.0.0:4000",
             Destination = HomeLocation,
             CurrentLocation = HomeLocation,
             OrderId = "",
             State = DroneState.Charging,
             Id = "abcdefg"
         };

         var response = await _droneGateway.AssignToFleet(newDrone.IpAddress, newDrone.BadgeNumber,
             newDrone.DispatcherUrl, newDrone.HomeLocation);
         if (!response.IsSuccessStatusCode)
         {
             Console.WriteLine("Drone is online, but there was a problem assigning to fleet. either the drone is not ready to be initialized or is already part of a fleet.");
             return false;
         }
         await _droneRepo.CreateAsync(newDrone);
         // Todo dispatcher saves handshake record to DB
         return true;
     }

    /// If a drone updates its status, patch its status.
    /// Then check if there is an enqueued order. If so,
    /// it should be assigned to this drone.
    [HttpPatch("update_status")]
    public async Task<bool> 
        PatchDroneStatus(DroneStatusPatch stateDto)
    {
        if (stateDto.State == DroneState.Ready && _unfilledOrders.Count > 0)
        {
            var orderDto = _unfilledOrders.Dequeue();
            _droneGateway.AssignDelivery(stateDto.Id, orderDto.OrderId, orderDto.OrderLocation);
        }
        return _orderRepo.PatchDroneStatus(stateDto).Result;
    }

    /// <summary>
    /// For testing
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<DroneRecord>> 
        Get(string id)
    {
        var droneRecord = await _droneRepo.GetAsync(id);

        if (droneRecord is null)
        {
            return NotFound();
        }
        return droneRecord;
    }
}