using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseAccess.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseAccess : ControllerBase
{
    

    private readonly ILogger<DatabaseAccess> _logger;
    private IFleetRepository _fleet;
    private IOrdersRepository _orders;
    
    public DatabaseAccess(
        ILogger<DatabaseAccess> logger, IFleetRepository fleet, IOrdersRepository orders)
    {
        _logger = logger;
        _fleet = fleet;
        _orders = orders;
    }

    [HttpGet("GetFleet")]
    public async Task<List<DroneRecord>> GetFleet()
    {
        Console.WriteLine("got a request to get the fleet...");
        var fleet =  await _fleet.GetAllAsync();
        Console.WriteLine("Got back" + string.Join("\n", fleet));
        return fleet;
    }

    [HttpGet("GetOrder")]
    public async Task<List<Order>> GetOrder()
    {
        Console.WriteLine("got a request to get the orders...");
        var orders = await _orders.GetAllAsync();
        Console.WriteLine("Got back" + string.Join("\n", orders));
        return orders;
    }

    [HttpPost("CreateOrder")]
    public async Task<CreateOrderResponse> CreateOrder(Order order)
    {
        await _orders.CreateAsync(order);
        return new CreateOrderResponse();
    }
    
    [HttpGet("GetDrone")]
    public DroneRecord GetDrone(string id)
    {
        return _fleet.GetByIdAsync(id).Result;
    }

    /*[HttpGet("GetOrder")]
    public Order GetOrder(string id)
    {
        return _orders.GetByIdAsync(id).Result;
    }*/
}
