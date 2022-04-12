using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.DTO;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways;

public class FrontEndToDispatchGateway : BaseGateway
{

    // Step 4, DroneToDispatchGateway takes in initial info
    // to create a GeoLocation and then POST its first status update 
    public async Task<string?> AddDrone(
    AddOrderRequest ready)
    {
        return await SendMessage(
            "AddDrone"
            , ready);
    }

    public void RemoveDrone(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        HttpClient = new HttpClient(handler);
    }

    public async Task<string?> SendDelivery(SendDeliveryRequest dto)
        => await SendMessage("SendDelivery", dto);
    
    public async Task<string?> CancelDeliveryRequest(string id)
        => await SendMessage("CancelDeliveryRequest",
            new CancelDeliveryRequest
            {
                OrderId = id
            });

    /// <summary>
    /// This method gets called when a drone updates its status.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<Task<Task<string?>>> PatchDroneStatus(DroneStatusUpdateRequest state)
        => Task.FromResult(SendMessage(Url, state));

    public FrontEndToDispatchGateway(string httpLocalhost) : base(httpLocalhost)
    {
    }
}