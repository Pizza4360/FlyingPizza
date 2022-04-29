using DatabaseAccess;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Scheduler.Controllers;

[ApiController]
[Route("[controller]")]
public class Scheduler
{
    private readonly FleetRepository _fleet;
    private readonly OrderRepository _orders;
    private readonly SchedulerToDispatchGateway _gateway;
    private const int RefreshInterval = 30000;
    private Timer _timer;
    private async void DequeueCallback(object _) => await TryDequeueOrders();

    [HttpPost("Initialize")]
    public async Task Initialize()
    {
        Console.WriteLine("Hello World!");
        while (true)
        {
            TryDequeueOrders();
            Thread.Sleep(RefreshInterval);
        }
    }
    public Scheduler()
    {
        // OrderRepository orders, FleetRepository fleet, SchedulerToDispatchGateway gateway
        _orders = new OrderRepository(new OrdersDatabaseSettings
        {
            CollectionName = "OrdersCompositeTest",
            ConnectionString = "mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/CapstoneGroup" ,
            DatabaseName = "Capstone"
        });
        _fleet = new FleetRepository(new FleetDatabaseSettings
        {
            CollectionName = "FleetCompositeTest",
            ConnectionString = "mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/CapstoneGroup" ,
            DatabaseName = "Capstone"
        });
        _gateway = new SchedulerToDispatchGateway();

        // _timer = new Timer(DequeueCallback, null, 0, RefreshInterval);
    }

    private async Task<string> TryDequeueOrders()
    {
        Console.WriteLine("Trying to dequeue some orders...");
        var orders = await GetUnfulfilledOrders();
        Console.WriteLine(string.Join("\n", orders.ToJson()));
        var availableDrones = GetAvailableDrones();
        Console.WriteLine(string.Join("\n", availableDrones.ToJson()));
        var enqueueOrderRequests = from drone in await availableDrones
            from order in orders
            where order.State == OrderState.Waiting
            select new EnqueueOrderRequest
            {
                OrderId = order.Id,
                OrderLocation = order.DeliveryLocation
            };
        var orderRequests = enqueueOrderRequests as EnqueueOrderRequest[] ?? enqueueOrderRequests.ToArray();
        Console.WriteLine($"Matched some drones with deliveries...\n{orderRequests.ToJson()}");
        var responseString = await _gateway.InitiateDeliveries(orderRequests);
        Console.WriteLine(responseString);
        return responseString;
    }
    private async Task<IEnumerable<Order>> GetUnfulfilledOrders()
    {
        var orders = from o in await _orders.GetAllAsync()
            where o != null && o.State == OrderState.Waiting
            select o;
        Console.WriteLine(string.Join(",", orders));
        return orders;
    }
    private async Task<IEnumerable<DroneRecord>> GetAvailableDrones()
    {
        Console.WriteLine("DequeueOrders...");
        var drones = from d in await _fleet.GetAllAsync()
            where d.State == DroneState.Ready && d.OrderId.Equals("")
            select d;
        return drones;
    }
}

public class SchedulerToDispatchGateway : BaseGateway<Scheduler>
{
    private string DispatchUrl{ get; } 
    public SchedulerToDispatchGateway(/*string ipAddress*/)
    {
        DispatchUrl = "http://localhost:83" + "/Dispatch";
    }
    public async Task<PingDto?> Ping(PingDto ready)
        => await SendMessagePost<PingDto, PingDto>($"{DispatchUrl}/Ping", new PingDto {
            S = "Malc"
        });

    public async Task<string?> InitiateDeliveries(IEnumerable<EnqueueOrderRequest> request) => 
        await SendMessagePost<IEnumerable<EnqueueOrderRequest>, string>
            ($"{DispatchUrl}/InitiateDeliveries",  request );
}