using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.GatewayDefinitions;
using SimDrone.Controllers;

namespace SimDrone;

public class DroneToDispatchGateway : BaseGateway<SimDroneController>
{
    public DroneToDispatchGateway(string dispatchUrl)
    {
        Console.WriteLine($"\n\n\n\nThis drone will talk to Dispatch at {dispatchUrl}\n\n\n\n");
        EndPoint = dispatchUrl + "/Dispatch";
    }

    public string EndPoint { get; }

    public void ChangeHandler(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        new HttpClient(handler);
    }

    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneUpdate request)
    {
        Console.WriteLine($"Drone is updating status to url {EndPoint}");
        return await SendMessagePost<DroneUpdate, UpdateDroneStatusResponse>
            ($"{EndPoint}/UpdateDroneStatus", request);
    }

    public async Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request)
    {
        return await SendMessagePost<CompleteOrderRequest, CompleteOrderResponse>(
            $"{EndPoint}/CompleteOrder", request);
    }
}