using Domain.DTO.DroneDispatchCommunication;
using Domain.GatewayDefinitions;
using SimDrone.Controllers;

namespace SimDrone;

public class DroneToDispatchGateway : BaseGateway<SimDroneController>
{
    public string Url => IpAndPort + "/Dispatch";

    public DroneToDispatchGateway(string ipAddress, int port) : base(port)
    {
        Console.WriteLine($"\n\n\n\nThis drone will talk to Dispatch at {ipAddress}{port}\n\n\n\n");
        IpAddress = ipAddress;
    }
    
    public void ChangeHandler(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        new HttpClient(handler);
    }
    
    public CompleteOrderResponse? CompleteOrder(CompleteOrderRequest request) => SendMessage<CompleteOrderRequest, CompleteOrderResponse>( 
        $"{Url}/CompleteOrder", request).Result;

    public UpdateDroneStatusResponse? UpdateDroneStatus(UpdateDroneStatusRequest request)
    {
        return SendMessage<UpdateDroneStatusRequest, UpdateDroneStatusResponse>
                ($"{Url}/UpdateDroneStatus", request).Result;
    }
}