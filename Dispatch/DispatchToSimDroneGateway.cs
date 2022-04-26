using Domain.DTO.DroneDispatchCommunication;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;
using MongoDB.Bson;
using DispatchController = Dispatch.Controllers.DispatchController;

namespace Dispatch;

public class DispatchToSimDroneGateway : BaseGateway<DispatchController>
{
    private ICompositeRepository _fleet;
    // private IOrdersRepository _orders;
    private async Task<string> Endpoint(string currentDroneId)
    {
        return (await _fleet.GetDroneByIdAsync(currentDroneId)).DroneUrl;
    }
    
    public DispatchToSimDroneGateway(ICompositeRepository fleet/*, IOrdersRepository orders*/)
    {
        _fleet = fleet;
        // _orders = orders;
    }
    
    public async Task<InitDroneResponse?> InitDrone(InitDroneRequest request)
    {
        var url = $"{request.DroneUrl}/SimDrone/InitDrone";
        Console.WriteLine($"\n\n\nurl:{url}\n\n");
        

        return await SendMessagePost<InitDroneRequest, InitDroneResponse>(
                url, request);
    }

    public async Task<AssignFleetResponse?> AssignFleet(AssignFleetRequest assignFleetRequest)
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
        
        var droneUrl = $"{assignFleetRequest.DroneIp}/SimDrone/AssignFleet";
        Console.WriteLine($"Sending {assignFleetRequest.DispatchIp} to the drone @ {droneUrl} so it can talk to us...");
        Console.WriteLine($"\n\n\n\n\n!!!!!!!!!!!assignFleetRequest.DispatchUrl = {assignFleetRequest.DispatchIp}\n\n\n\n\n");

        var response = SendMessagePost<AssignFleetRequest, AssignFleetResponse>(
            droneUrl, assignFleetRequest);


        response.Wait();
        return response.Result;
    }

    public async Task<AssignDeliveryResponse?> AssignDelivery(AssignDeliveryRequest assignDeliveryRequest)
    {
        var url = await Endpoint(assignDeliveryRequest.DroneUrl);
        Console.WriteLine($"\n\nChoosing drone {assignDeliveryRequest.DroneUrl} url:{url}\n\n\n");
        return await SendMessagePost<AssignDeliveryRequest, AssignDeliveryResponse>(
            $"{url}/SimDrone/AssignDelivery", assignDeliveryRequest);
    }
}