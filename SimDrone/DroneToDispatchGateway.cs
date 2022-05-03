using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using SimDrone.Controllers;

namespace SimDrone;

public class DroneToDispatchGateway : BaseGateway<SimDroneController>
{
    public DroneToDispatchGateway(string dispatchUrl)
    {
        Console.WriteLine($"\nThis drone will talk to Dispatch at {dispatchUrl}");
        EndPoint = dispatchUrl + "/Dispatch";
    }

    public string EndPoint { get; set; }

    public void ChangeHandler(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        new HttpClient(handler);
    }

    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneUpdate droneUpdate)
    {
        Console.WriteLine($"Drone is updating status to url {EndPoint}");
        return await SendMessagePost<DroneUpdate, UpdateDroneStatusResponse>
            ($"{EndPoint}/UpdateDroneStatus", droneUpdate);
    }

    public async Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request)
    {
        return await SendMessagePost<CompleteOrderRequest, CompleteOrderResponse>(
            $"{EndPoint}/CompleteOrder", request);
    }

    public async Task<bool> Recover(DroneRecord record)
    {
        return await SendMessagePost<DroneRecord, bool>(
            $"{EndPoint}/Recover", record);
    }
}