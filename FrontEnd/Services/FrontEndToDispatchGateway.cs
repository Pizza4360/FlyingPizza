using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.GatewayDefinitions;

namespace FrontEnd.Services;

public class FrontEndToDispatchGateway : BaseGateway<App>
{
    private string DispatchUrl{ get; } 
    public FrontEndToDispatchGateway(/*string ipAddress*/)
    {
        DispatchUrl = "http://localhost:83" + "/Dispatch";
    }
    
    public async Task<PingDto> Ping(PingDto ready)
        => await SendMessagePost<PingDto, PingDto>($"{DispatchUrl}/Ping", new PingDto {
                    S = "Malc"
                });

    public async Task<EnqueueOrderResponse> EnqueueOrder(EnqueueOrderRequest request) => 
        await SendMessagePost<EnqueueOrderRequest, EnqueueOrderResponse>($"{DispatchUrl}/EnqueueOrder",  request );

    /*public void RemoveDrone(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        // TODO: what why?
        // HttpClient = new HttpClient(handler);
    }*/


    public async Task<AddDroneResponse> AddDrone(AddDroneRequest request) 
        => await SendMessagePost<AddDroneRequest, AddDroneResponse>($"{DispatchUrl}/AddDrone", new AddDroneRequest {
        DroneId = request.DroneId,
        DroneUrl = request.DroneUrl,
        DispatchUrl = DispatchUrl
        });


    public async Task<CancelDeliveryResponse> CancelDelivery(string id) =>
          await SendMessagePost<CancelDeliveryRequest, CancelDeliveryResponse>($"{DispatchUrl}/CancelDelivery",
            new CancelDeliveryRequest
            {
                OrderId = id
            });
}

