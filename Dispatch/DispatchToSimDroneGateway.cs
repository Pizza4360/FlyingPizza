using Domain.DTO.DroneDispatchCommunication;
using Domain.GatewayDefinitions;
using DispatchController = Dispatch.Controllers.DispatchController;

namespace Dispatch;

public class DispatchToSimDroneGateway : BaseGateway<DispatchController>
{ 
    private string Url(string currentDroneId) => IdToIpDictionary[currentDroneId] + "/SimDrone";
    
    private Dictionary<string, string> IdToIpDictionary { get; }

    public DispatchToSimDroneGateway(Dictionary<string, string> dictionary, int port) : base(port)
    {
        IdToIpDictionary = dictionary;
    }
    
    public InitDroneResponse?
        InitDrone(InitDroneRequest request)
        =>  SendMessage<InitDroneRequest, InitDroneResponse>(
            $"{request.DroneIp}/SimDrone/InitDrone",
            request).Result;


    public AssignFleetResponse? AssignFleet(AssignFleetRequest assignFleetRequest)
    {
        if(assignFleetRequest.DispatchIp is null or "")
        {
            Console.WriteLine("A Dispatch Ip Address is requred!");

            return new AssignFleetResponse
            {
                DroneId = assignFleetRequest.DroneId,
                IsInitializedAndAssigned = false
            };
        }

        IdToIpDictionary[assignFleetRequest.DroneId] = assignFleetRequest.DroneIp;

        var droneUrl = $"{assignFleetRequest.DroneIp}/SimDrone/AssignFleet";
        Console.WriteLine($"Sending {assignFleetRequest.DispatchIp} to the drone so it can talk to us...");

        Console.WriteLine($"\n\n\n\n\n!!!!!!!!!!!assignFleetRequest.DispatchIp = {assignFleetRequest.DispatchIp}\n\n\n\n\n");

        var response = SendMessage<AssignFleetRequest, AssignFleetResponse>(
            $"{droneUrl}", assignFleetRequest);


        response.Wait();
        return response.Result;
    }

    public AssignDeliveryResponse? AssignDelivery(AssignDeliveryRequest assignDeliveryRequest)
    {
        var url = Url(assignDeliveryRequest.DroneId);
        Console.WriteLine($"\n\nChoosing drone {assignDeliveryRequest.DroneId} url:{url}\n\n\n");
        return SendMessage<AssignDeliveryRequest, AssignDeliveryResponse>(
                $"{url}/AssignDelivery", assignDeliveryRequest)
           .Result;
    }
}