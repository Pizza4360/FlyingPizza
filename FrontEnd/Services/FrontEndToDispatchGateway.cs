﻿using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
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
    

    public async Task<AddDroneResponse> AddDrone(string droneUrl)
    {
        return await SendMessagePost<AddDroneRequest, AddDroneResponse>($"{DispatchUrl}/AddDrone", new AddDroneRequest
        {
            DroneUrl = droneUrl,
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