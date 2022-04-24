using Domain.DTO.DroneDispatchCommunication;
using Domain.GatewayDefinitions;
using SimDrone.Controllers;

namespace SimDrone;

public class DroneToDispatchGateway : BaseGateway<SimDroneController>
{
    public string EndPoint{get;}

    public DroneToDispatchGateway(string dispatchUrl)
    {
        Console.WriteLine($"\n\n\n\nThis drone will talk to Dispatch at {dispatchUrl}\n\n\n\n");
        EndPoint = dispatchUrl + "/Dispatch";
    }
    
    public void ChangeHandler(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        new HttpClient(handler);
    }
    
    public async Task<CompleteOrderResponse?> CompleteOrder(CompleteOrderRequest request) => await SendMessage<CompleteOrderRequest, CompleteOrderResponse>( 
        $"{EndPoint}/CompleteOrder", request);

    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(UpdateDroneStatusRequest request)
    {
        Console.WriteLine($"Drone is updating status to url {EndPoint}");
        return await SendMessage<UpdateDroneStatusRequest, UpdateDroneStatusResponse>
                ($"{EndPoint}/UpdateDroneStatus", request);
    }
}