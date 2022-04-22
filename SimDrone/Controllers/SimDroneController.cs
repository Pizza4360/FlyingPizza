using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace SimDrone.Controllers;

[ApiController]
[Route("[controller]")]
public class SimDroneController : ControllerBase
{
    private static Drone _drone;
    private static DroneToDispatchGateway _gateway;
    private bool IsInitiatead = false;

    [HttpPost("InitDrone")]
    public InitDroneResponse InitDrone(
        InitDroneRequest initDroneRequest)
    {
        var responseString = IsInitiatead ? "This drone is already initialized" : "This drone is ready to be initialized to a fleet.";
        Console.WriteLine($"SimDroneController.InitDrone -> {initDroneRequest}\tresponse -> {responseString}");
        return new InitDroneResponse
        {
            DroneId = initDroneRequest.DroneId,
            Okay = !IsInitiatead
        };
    }
        
        
    [HttpPost("AssignFleet")]
    public AssignFleetResponse AssignFleet(AssignFleetRequest assignFleetRequest)
    {
        Console.WriteLine($"SimDroneController.AssignFleet -> {assignFleetRequest}");
        _gateway = new DroneToDispatchGateway(assignFleetRequest.DispatchIp, 84);
        _drone = new Drone(new DroneRecord
            {
                BadgeNumber = assignFleetRequest.BadgeNumber,
                CurrentLocation = assignFleetRequest.HomeLocation,
                Destination = assignFleetRequest.HomeLocation,
                DispatchIp = assignFleetRequest.DispatchIp,
                DroneIp = assignFleetRequest.DroneIp,
                HomeLocation = assignFleetRequest.HomeLocation,
                DroneId = assignFleetRequest.DroneId,
                OrderId = null
            }, _gateway, 
            this);
        Console.WriteLine($"SimDrone successfully initialized.\nDrone -->{_drone}");
        IsInitiatead = true;
        return new AssignFleetResponse
        {
            FirstState = DroneState.Ready,
            DroneId = assignFleetRequest.DroneId,
            IsInitializedAndAssigned = true
        };
    }

        
    [HttpPost("AssignDelivery")]
    public AssignDeliveryResponse AssignDelivery(
        AssignDeliveryRequest assignDeliveryRequest)
    {
        Console.WriteLine($"SimDroneController.AssignDelivery -> {assignDeliveryRequest}");
        return _drone.AssignDelivery(assignDeliveryRequest);
    }


    [NonAction]
    public UpdateDroneStatusResponse? UpdateDroneStatus(UpdateDroneStatusRequest updateDroneStatusRequest)
        => _gateway.UpdateDroneStatus(updateDroneStatusRequest);
}