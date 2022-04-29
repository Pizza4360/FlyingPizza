using System.Net.Http.Headers;
using DatabaseAccess;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.DTO.SchedulerDispatch;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Scheduler.Controllers;
/*

[ApiController]
[Route("[controller]")]
public class Scheduler
{
    private readonly FleetRepository _fleet;
    private readonly OrderRepository _orders;
    private readonly SchedulerToDispatchGateway _gateway;
    private const int RefreshInterval = 200000;
    private HttpClient _httpClient;


    [HttpPost("Initialize")]
    public void Initialize()
    {
        while (true)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var r = _httpClient.PostAsJsonAsync("http://localhost:83/Dispatch/Ping", new PingDto{S = "TRY ME"});
            Thread.Sleep(RefreshInterval);
        }
    }
    public Scheduler()
    {


    // private async Task TryDequeueOrders()
    // {
    //     Console.WriteLine("Trying to dequeue some orders...");
    //     var orders = await GetUnfulfilledOrders();
    //     Console.WriteLine(string.Join("\n", orders.ToJson()));
    //     var availableDrones = GetAvailableDrones();
    //     Console.WriteLine(string.Join("\n", availableDrones.ToJson()));
    //     var assignments = from drone in await availableDrones
    //         from order in orders
    //         where order.State == OrderState.Waiting
    //         select new AssignDeliveryRequest
    //         {
    //             OrderId = order.Id,
    //             OrderLocation = order.DeliveryLocation,
    //             DroneId = drone.DroneId
    //         };
    //     foreach (var delivery in assignments)
    //     {
    //         Console.WriteLine($"Matched some drones with deliveries...\n{assignments.ToJson()}");
    //         var responseString = await _gateway.AssignDelivery(delivery);
    //         Console.WriteLine(responseString.ToJson());
    //     }
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


    public async Task<AssignDeliveryResponse> AssignDelivery(AssignDeliveryRequest request)
    {
        return await SendMessagePost
            <AssignDeliveryRequest, AssignDeliveryResponse>
            ($"{DispatchUrl}/AssignDelivery", request);
    }
}*/

