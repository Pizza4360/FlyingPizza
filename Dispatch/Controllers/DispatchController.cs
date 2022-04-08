using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Dispatch.Controllers;

[ApiController]
[Route("[controller]")]
public class DispatchController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<DispatchController> _logger;
    private readonly IDronesRepository _dronesRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IDroneGateway _droneGateway;
    private readonly Queue<Order> _unfilledOrders;
    private readonly GeoLocation _home;

    public DispatchController(
        IDronesRepository droneRepository,
        IOrdersRepository ordersRepository,
        IDroneGateway droneGateway,
        GeoLocation home
        )
    {
        Console.WriteLine("Hello world, from dispatch!");
        _unfilledOrders = new Queue<Order>();
        _dronesRepository = droneRepository;
        _ordersRepository = ordersRepository;
        _droneGateway = droneGateway;
        _home = /*home*/ new GeoLocation
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00561147600774m
        };
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    
    [HttpPost("add_order")]
    public async Task<IActionResult> AddNewOrder(AddOrderDTO order)
    {
        Console.WriteLine("adding a new order");
        bool didSucceed;
        var availableDrones = await _dronesRepository.GetAllAvailableDronesAsync();
            
        var newOrder = await _ordersRepository.GetByIdAsync(order.DroneId);
        if (availableDrones.Any())
        {
            didSucceed = await _droneGateway.AssignDelivery(availableDrones.First().IpAddress, newOrder.Id,
                newOrder.DeliveryLocation);
        }
        else
        {
            _unfilledOrders.Enqueue(newOrder);
            didSucceed = true;
        }

        // TODO: unhappy path
        Console.WriteLine($"DispatcherController.AddNewOrder({order}) - Order Complete"); // Debug
        return Ok(didSucceed);
    }
}