using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;

namespace Scheduler;

public class Scheduler
{
    private readonly IFleetRepository _fleet;
    private readonly IOrdersRepository _orders;
    private readonly SchedulerToDispatchGateway _gateway;
    private const int RefreshInterval = 2000;
    private Timer _timer;
    private async void DequeueCallback(object _) => await TryDequeueOrders();

    public async Task Initialize()
    {
        _timer = new Timer(DequeueCallback, null, 0, RefreshInterval);
    }

    public Scheduler(IOrdersRepository orders, IFleetRepository fleet, SchedulerToDispatchGateway gateway)
    {
        _orders = orders;
        _fleet = fleet;
        _gateway = gateway;
        if (_timer == null)
        {
            Initialize();
        }
    }

    private async Task<string> TryDequeueOrders()
    {
        Console.WriteLine("Trying to dequeue some orders...");
        var orders = await GetUnfulfilledOrders();
        var responseString = await _gateway.TryDequeueOrders(from drone in await GetAvailableDrones()
            from order in orders
            select new EnqueueOrderRequest
            {
                OrderId = order.Id,
                OrderLocation = order.DeliveryLocation
            });
        Console.WriteLine(responseString);
        return responseString;
    }
    private async Task<IEnumerable<Order>> GetUnfulfilledOrders()
    {
        var orders = from o in await _orders.GetAllAsync()
            where !o.HasBeenDelivered && o.DroneId.Equals(string.Empty)
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

    public async Task<string?> TryDequeueOrders(IEnumerable<EnqueueOrderRequest> request) => 
        await SendMessagePost<IEnumerable<EnqueueOrderRequest>, string>
            ($"{DispatchUrl}/TryDequeueOrders",  request );
}