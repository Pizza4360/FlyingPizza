using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.GatewayDefinitions;

namespace FrontEnd.Services;

public class FrontEndToDispatchGateway : BaseGateway<App>
{
    public FrontEndToDispatchGateway(string dispatchUrl)
    {
        DispatchUrl = dispatchUrl;
    }

    private string DispatchUrl { get; }

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

    /*public void RemoveDrone(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        // TODO: what why?
        // HttpClient = new HttpClient(handler);
    }*/


    public async Task<AddDroneResponse> AddDrone(AddDroneRequest request)
    {
        return await SendMessagePost<AddDroneRequest, AddDroneResponse>($"{DispatchUrl}/AddDrone", request);
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