using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SimDrone.Gateways;

namespace SimDrone.Controllers;

[ApiController]
[Route("[controller]")]
public class SimDroneController : ControllerBase
{
    private Drone _drone;

    /// <summary>
    /// Command a drone to deliver an order.
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    [HttpPost("deliver")]
    public async Task<IActionResult> DeliverOrder(Delivery order)
    {
        Console.WriteLine($"Delivering {order}");
        _drone.DeliverOrder(order.OrderLocation);
        return Ok();
    }
    
    /// <summary>
    /// For testing purposes.
    /// </summary>
    /// <returns>"hello, world"</returns>
    [HttpPost("ping")]
    public async Task<string> Ping()
    {
        return "hello, world";
    }
    
    /// <summary>
    /// This method is called when a drone has been pinged to be
    /// initialized into a fleet. SimDroneController is idle until
    /// this method is called.
    /// </summary>
    /// <returns></returns>
    [HttpPost("initregistration")]
    public async Task<IActionResult> InitializeRegistration()
    {
        Console.WriteLine($"Initializing{_drone}");
        return Ok();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="record"></param>
    /// <param name="gateway"></param>
    /// <returns></returns>
    [HttpPost("completeregistration")]
    public async Task<IActionResult> CompleteRegistration(DroneRecord record, DispatcherGateway gateway)
    {
        Console.WriteLine("Generating simulated drone...");
        _drone = new Drone(record, gateway);
        // TODO: initialize this from the drones repository, based on a drone id from environment variables
        var doneString = $"SimDrone successfully initialized.\nDrone -->{_drone}";
        Console.WriteLine(doneString);
        return Ok(doneString);
    }
    
    public void changeDrone(Drone drone)
    {
        _drone = drone;
    }
}