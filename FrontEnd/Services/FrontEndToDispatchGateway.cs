using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.GatewayDefinitions;

namespace FrontEnd.Services;

public class FrontEndToDispatchGateway : BaseGateway<App> 
{
    private string _url;
    public string Url => IpAndPort +  "/Dispatch";
    
    public FrontEndToDispatchGateway(string ipAddress, int port) : base(port)
    {
        IpAddress = ipAddress;
    }
    
    public PingDto? Ping(PingDto ready)
    {
        return SendMessage<PingDto, PingDto>($"{Url}/Ping", new PingDto { S = "Malc" }).Result;
    }

    public AddOrderResponse? AddOrder(AddOrderRequest ready)
    => SendMessage<AddOrderRequest, AddOrderResponse>
            ($"{Url}/EnqueueOrder", ready).Result;


    public RemoveDroneResponse? RemoveDrone(RemoveDroneRequest request)
        => SendMessage<RemoveDroneRequest, RemoveDroneResponse>
            ($"{Url}/RemoveDrone", request).Result;


    public AddDroneResponse? AddDrone(AddDroneRequest request)
        => SendMessage<AddDroneRequest, AddDroneResponse>($"{Url}/AddDrone", request).Result;


    public CancelDeliveryResponse? CancelDeliveryRequest(string id) =>
        SendMessage<CancelDeliveryRequest, CancelDeliveryResponse>(
            $"{Url}/CancelDeliveryRequest",
            new CancelDeliveryRequest
            {
                OrderId = id
            }).Result;
}
