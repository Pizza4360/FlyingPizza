using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Implementation.Gateways;
using Microsoft.AspNetCore.Mvc;

namespace SimDrone.Controllers;

[ApiController]
[Route("[controller]")]
public class SimDroneController : ControllerBase
{
    private readonly Drone _drone;

    public SimDroneController(DroneToDispatcherGateway droneToDispatcherGateway)
    {
        Console.WriteLine("made it \n\n\n\n!!!!!!!!!!!!!");
        _drone = new Drone("123", new GeoLocation
            {
                Latitude = 39.74364421910773m,
                Longitude = -105.00561147600774m
            }, droneToDispatcherGateway,
            5, "1", "something"); // TODO: initialize this from the drones repository, based on a drone id from environment variables
        Console.WriteLine(_drone);
    }

    [HttpPost("deliver")]
    public async Task<IActionResult> Deliver(DeliverOrderDto order)
    {
        Console.WriteLine($"Delivering {order}");
        _drone.DeliverOrder(order.OrderLocation);
        return Ok();
    }
    
    [HttpPost("ping")]
    public async Task<string> Ping(string s)
    {
        return $"hello, {s}";
    }
    
    //
    // [HttpPost("initregistration")]
    // public async Task<IActionResult> InitializeRegistration()
    // {
    //     Console.WriteLine($"Initializing{_drone}");
    //     // Todo, add logic to verify legitimacy of adding a drone.
    //     return Ok("SimDrone successfully initialized");
    // }
    //
    // [HttpPost("completeregistration")]
    // public async Task<IActionResult> CompleteRegistration()
    // {
    //     return Ok();
    // }
    //
    // public void changeDrone(SimDrone drone)
    // {
    //     _drone = drone;
    // }
}