using Dispatch.Controllers;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;

namespace Dispatch;

public class DispatchToSimDroneGateway : BaseGateway<DispatchController>
{
    private readonly IFleetRepository _fleet;

    public DispatchToSimDroneGateway(IFleetRepository fleet /*, IOrdersRepository orders*/)
    {
        _fleet = fleet;
        // _orders = orders;
    }

    // private IOrdersRepository _orders;
    private async Task<string> Endpoint(string currentDroneId)
    {
        return (await _fleet.GetByIdAsync(currentDroneId)).DroneUrl;
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
        if (assignFleetRequest.DispatchUrl is null or "")
        {
            Console.WriteLine("A Dispatch Ip Address is requred!");
            return new AssignFleetResponse
            {
                DroneId = assignFleetRequest.DroneId,
                IsInitializedAndAssigned = false
            };
        }

        var droneUrl = $"{assignFleetRequest.DroneIp}/SimDrone/AssignFleet";
        Console.WriteLine($"Sending {assignFleetRequest.DispatchUrl} to the drone @ {droneUrl} so it can talk to us...");
        Console.WriteLine(
            $"\n\n\n\n\n!!!!!!!!!!!assignFleetRequest.DispatchUrl = {assignFleetRequest.DispatchUrl}\n\n\n\n\n");

        var response = SendMessagePost<AssignFleetRequest, AssignFleetResponse>(
            droneUrl, assignFleetRequest);


        response.Wait();
        return response.Result;
    }

    public async Task<AssignDeliveryResponse?> AssignDelivery(AssignDeliveryRequest assignDeliveryRequest)
    {
        var url = await Endpoint(assignDeliveryRequest.DroneId);
        Console.WriteLine($"\n\nChoosing drone {assignDeliveryRequest.DroneId} url:{url}\n\n\n");
        return await SendMessagePost<AssignDeliveryRequest, AssignDeliveryResponse>(
            $"{url}/SimDrone/AssignDelivery", assignDeliveryRequest);
    }

    public async Task<bool> HealthCheck(string droneUrl)
    {
        try
        {
            var url = await Endpoint(droneUrl);
            await SendMessagePost<PingDto, DroneRecord>(droneUrl, new PingDto {S = "revive"});
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}