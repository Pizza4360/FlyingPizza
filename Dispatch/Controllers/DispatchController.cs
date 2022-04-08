using Dispatch.Repositories;
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
    private readonly FleetService _service;
    private readonly OrdersService _ordersService;
    private readonly ILogger<DispatchController> _logger;
  

    public DispatchController(FleetService service, OrdersService ordersService)
    {
        Console.WriteLine(DateTime.Now);
        _service = service;
        _ordersService = ordersService;
        /*Order o = new Order()
        {
            CustomerName = "malc",
            DeliveryAddress = "444 some place",
            DeliveryLocation = new GeoLocation()
            {
                Latitude = 39.743787586026905m,
                Longitude = -105.00333787196135m
            },
            Id = "myId",
            Items = new object[2],
            TimeOrdered = DateTime.Now,
            URL = "https://blah",
        };
        Console.WriteLine("putting order!!!!!!");
        _ordersService.AddOrder(o);*/
        
    }

    [HttpPut]
    public async Task<bool> Put()
    {
        Order o = new Order()
        {
            CustomerName = "malc",
            DeliveryAddress = "444 some place",
            DeliveryLocation = new GeoLocation
            {
                Latitude = 39.743787586026905m,
                Longitude = -105.00333787196135m
            },
            Id = "myId",
            Items = new object[2],
            TimeOrdered = DateTime.Now,
            URL = "https://blah"
        };
        return _ordersService.AddOrder(o).Result;
    }
    // [HttpGet]
    // public async Task<List<Order>> Get() => await _service.GetAsync();
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Order>> Get(string id)
    {
        var droneRecord = await _ordersService.GetAsync(id);

        if (droneRecord is null)
        {
            return NotFound();
        }
        return droneRecord;
    }
    /*[HttpGet("{id:length(24)}")]
    public async Task<ActionResult<DroneRecord>> Get(string id)
    {
        var droneRecord = await _service.GetAsync(id);

        if (droneRecord is null)
        {
            return NotFound();
        }
        return droneRecord;
    }*/
    [HttpPost("ping")]
    public async Task<string> Ping(string s)
    {
        return $"hello, {s}";
    }

   
}