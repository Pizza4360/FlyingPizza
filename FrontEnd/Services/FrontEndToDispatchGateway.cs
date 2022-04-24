using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.GatewayDefinitions;

namespace FrontEnd.Services;

public class FrontEndToDispatchGateway : BaseGateway<App> 
{
    public string Url{get;}

    public FrontEndToDispatchGateway(string url)
    {
        Url = url;
    }
    
    public async Task<PingDto?> Ping(PingDto ready)
    {
        return await SendMessage<PingDto, PingDto>($"{Url}/Ping", new PingDto { S = "Malc" });
    }

    public async Task<AddOrderResponse?> AddOrder(AddOrderRequest ready)
    => await SendMessage<AddOrderRequest, AddOrderResponse>
            ($"{Url}/EnqueueOrder", ready);


    public async Task<RemoveDroneResponse?> RemoveDrone(RemoveDroneRequest request)
        => await SendMessage<RemoveDroneRequest, RemoveDroneResponse>
            ($"{Url}/RemoveDrone", request);


    public async Task<AddDroneResponse?> AddDrone(AddDroneRequest request)
        => await SendMessage<AddDroneRequest, AddDroneResponse>($"{Url}/AddDrone", request);


    public async Task<CancelDeliveryResponse?> CancelDeliveryRequest(string id) =>
        await SendMessage<CancelDeliveryRequest, CancelDeliveryResponse>(
            $"{Url}/CancelDeliveryRequest",
            new CancelDeliveryRequest
            {
                OrderId = id
            });
}
