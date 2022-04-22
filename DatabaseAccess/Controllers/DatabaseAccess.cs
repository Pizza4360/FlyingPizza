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
    public DroneRecord[] GetFleet()
    {
        return _fleet.GetAllAsync().Result.ToArray();
    }
    
    [HttpPost("AddOrder")]
    public async Task<IActionResult> AddOrder(Order order)
    {
        await _orders.CreateAsync(order);
        return Ok();
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
