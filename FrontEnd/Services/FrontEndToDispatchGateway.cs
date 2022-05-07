using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;

namespace FrontEnd.Services;

public class FrontEndToDispatchGateway : BaseGateway<App>
{
    private string DispatchUrl { get; }

    public FrontEndToDispatchGateway(string dispatchUrl)
    {
        DispatchUrl = dispatchUrl;
    }

    public async Task<PingDto> Ping(PingDto ready)
    {
        return await SendMessagePost<PingDto, PingDto>($"{DispatchUrl}/Ping", new PingDto
        {
            S = "Malc"
        });
    }

    public async Task<EnqueueOrderResponse> EnqueueOrder(EnqueueOrderRequest request)
    {
        return await SendMessagePost<EnqueueOrderRequest, EnqueueOrderResponse>($"{DispatchUrl}/EnqueueOrder", request);
    }

    public async Task<AddDroneResponse> AddDrone(string droneUrl)
    {
        Console.WriteLine($"drone url is {droneUrl}, dispatchUrl = {DispatchUrl}");
        return await SendMessagePost<AddDroneRequest, AddDroneResponse>($"{DispatchUrl}/AddDrone",
            new AddDroneRequest
        {
            DroneUrl = droneUrl,
            DroneId = BaseEntity.GenerateNewId(),
            DispatchUrl = "",
            HomeLocation = null
        });
    }

    public async Task<CancelDeliveryResponse> CancelDelivery(string id)
    {
        return await SendMessagePost<CancelDeliveryRequest, CancelDeliveryResponse>($"{DispatchUrl}/CancelDelivery",
            new CancelDeliveryRequest
            {
                OrderId = id
            });
    }
}