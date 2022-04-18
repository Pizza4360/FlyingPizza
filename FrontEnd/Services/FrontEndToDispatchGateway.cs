using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;

namespace FrontEnd.Services;

public class FrontEndToDispatchGateway : BaseGateway
{
    public PingDto Ping(PingDto ready)
    {
        return (PingDto) SendMessage("", "Ping",
            new PingDto()
            {
                S = "Malc"
            });
    }

    public AddOrderResponse AddOrder(AddOrderRequest ready) => (AddOrderResponse) SendMessage("", "AddOrder", ready);

    public void RemoveDrone(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        // TODO: what why?
        HttpClient = new HttpClient(handler);
    }


    public AddDroneResponse AddDrone(AddDroneRequest request) 
        => (AddDroneResponse)SendMessage("", "", new InitDroneRequest
    {
        DroneId = request.DroneIp,
        DroneIp = request.DroneIp,
        Id = request.Id
    });


    public CancelDeliveryResponse CancelDeliveryRequest(string id) =>
        (CancelDeliveryResponse) SendMessage(
            "",
            "CancelDeliveryRequest",
            new CancelDeliveryRequest
            {
                OrderId = id
            });

    
    public DroneStatusUpdateResponse PatchDroneStatus(DroneStatusUpdateRequest state) 
        => (DroneStatusUpdateResponse)SendMessage("", Url, state);
}

public class PingDto
    : BaseDto
{
    public string S { get; set; }
}
