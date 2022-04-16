using DatabaseAccess.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseAccess.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseAccess : ControllerBase
{
    

    private readonly ILogger<DatabaseAccess> _logger;
    private FleetRepository _fleet;
    // private OrderRepository _orders;
    
    public DatabaseAccess(
    ILogger<DatabaseAccess> logger, FleetRepository fleet/*, OrderRepository orders*/)
    {
        _logger = logger;
        _fleet = fleet;
        // _orders = orders;
    }
    [HttpGet("GetFleet")]
    public DroneRecord[] GetFleet()
    {
        return _fleet.GetAll().Result.ToArray();
    }
    //
    // [HttpPost("AddOrder")]
    // public OkObjectResult AddOrder(
    // Order order) => Ok( _orders.CreateAsync(order) .Result);
    
    [HttpGet("GetDrone")]
    public DroneRecord GetDrone(string id)
    {
        return _fleet.GetByIdAsync(id).Result;
    }

    // [HttpGet("GetOrder")]
    // public Order GetOrder(string id)
    // {
    //     return _orders.GetByIdAsync(id).Result;
    // }
}
