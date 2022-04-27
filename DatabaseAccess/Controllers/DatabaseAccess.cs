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
    private readonly ILogger<DatabaseAccess> _logger;
    private ICompositeRepository _repository;
    private ICompositeRepository _composite;
    
    public DatabaseAccess(
        ILogger<DatabaseAccess> logger, ICompositeRepository repository, ICompositeRepository composite)
    {
        _logger = logger;
        _repository = repository;
        _composite = composite;
    }

    [HttpGet("GetFleet")]
    public async Task<List<DroneRecord>> GetFleet()
    {
        Console.WriteLine("got a request to get the repository...");
        var fleet =  await _repository.GetDrones();
        Console.WriteLine("Got back" + string.Join("\n", fleet));
        return fleet;
    }
    
    [HttpPost("CreateOrder")]
    public async Task<CreateOrderResponse> CreateOrder(Order order)
    {
        // await _composite.CreateAsync(order);
        await _composite.EnqueueOrder(order);
        return new CreateOrderResponse();
    }
    
    [HttpGet("GetDrone")]
    public DroneRecord GetDrone(string id)
    {
        return _repository.GetDroneByIdAsync(id).Result;
    }

    [HttpGet("GetOrder")]
    public async Task<Order> GetOrder(string id) =>await _composite.GetOrderByIdAsync(id);
}
