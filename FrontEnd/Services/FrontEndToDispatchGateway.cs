using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;

namespace FrontEnd.Services;

public class FrontEndToDispatchGateway : BaseGateway
{
    public async Task Ping( Ping ready)
    => SendMessage("Ping", new Ping() { S = "Malc" });


    // Step 4, DroneToDispatchGateway takes in initial info
    // to create a GeoLocation and then POST its first status update 
    public async Task<AddOrderResponse> AddOrder(AddOrderRequest ready)
    => (AddOrderResponse)SendMessage("AddOrder", ready);
    

    public async Task<AddDroneResponse> AddDrone(AddDroneRequest request)
    =>  (AddDroneResponse)SendMessage("InitDrone", request);

    public void RemoveDrone(
    HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        // TODO: what why?
        HttpClient = new HttpClient(handler);
    }

    public async Task<CancelDeliveryResponse> CancelDeliveryRequest(
    string id) => (CancelDeliveryResponse)SendMessage(
        "CancelDeliveryRequest",
        new CancelDeliveryRequest { OrderId = id });
}