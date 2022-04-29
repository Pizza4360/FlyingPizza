using Domain.DTO.DroneDispatchCommunication;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;
using MongoDB.Bson;
using DispatchController = Dispatch.Controllers.DispatchController;

namespace Dispatch;

public class DispatchToSimDroneGateway : BaseGateway<DispatchController>
{
    private IFleetRepository _fleet;
    private async Task<string> Endpoint(string currentDroneId)
    {
        return (await _fleet.GetByIdAsync(currentDroneId)).DroneUrl;
    }
    
    public DispatchToSimDroneGateway(IFleetRepository fleet)
    {
        _fleet = fleet;
    }
    
    public async Task<InitDroneResponse?> InitDrone(InitDroneRequest request)
    {
        var url = $"{request.DroneUrl}/SimDrone/InitDrone";
        return await SendMessagePost<InitDroneRequest, InitDroneResponse>(
                url, request);
    }

    public async Task<AssignFleetResponse?> AssignFleet(AssignFleetRequest assignFleetRequest)
    {
        if(string.IsNullOrWhiteSpace(assignFleetRequest.DispatchIp))
        {
            return new AssignFleetResponse
            {
                DroneId = assignFleetRequest.DroneId,
                IsInitializedAndAssigned = false
            };
        }
        
        var droneUrl = $"{assignFleetRequest.DroneIp}/SimDrone/AssignFleet";

        var response = await SendMessagePost<AssignFleetRequest, AssignFleetResponse>(
            droneUrl, assignFleetRequest);

        return response;
    }

    public async Task<AssignDeliveryResponse?> AssignDelivery(AssignDeliveryRequest assignDeliveryRequest)
    {
        var url = await Endpoint(assignDeliveryRequest.DroneId);
        Console.WriteLine($"\n\nChoosing drone {assignDeliveryRequest.DroneId} url:{url}\n\n\n");
        return await SendMessagePost<AssignDeliveryRequest, AssignDeliveryResponse>(
            $"{url}/SimDrone/AssignDelivery", assignDeliveryRequest);
    }
}