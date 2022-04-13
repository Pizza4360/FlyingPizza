using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;

namespace FrontEnd.Services;

public class FrontEndToDispatchGateway : BaseGateway
{
    public async Task<string?> Ping(
    PingDto ready)
    {
        return await SendMessage(
            "Ping",
            new PingDto()
            {
                S = "Malc"
            });
    }


    // Step 4, DroneToDispatchGateway takes in initial info
    // to create a GeoLocation and then POST its first status update 
    public async Task<string?> AddOrder(
    AddOrderRequest ready)
    {
        return await SendMessage("AddOrder", ready);
    }

    public void RemoveDrone(
    HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        // TODO: what why?
        HttpClient = new HttpClient(handler);
    }

    public async Task<string?> SendDelivery(
    SendDeliveryRequest dto) =>
        await SendMessage("SendDelivery", dto);

    public async Task<string?> CancelDeliveryRequest(
    string id) =>
        await SendMessage(
            "CancelDeliveryRequest",
            new CancelDeliveryRequest
            {
                OrderId = id
            });

    /// <summary>
    /// This method gets called when a drone updates its status.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<string?> PatchDroneStatus(
    DroneStatusUpdateRequest state) =>
        SendMessage(Url, state).Result;
}

public class PingDto
    : BaseDTO
{
    public string S { get; set; }
}
