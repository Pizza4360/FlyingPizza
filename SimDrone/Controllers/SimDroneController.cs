using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace SimDrone.Controllers;

[ApiController]
[Route("[controller]")]
public class SimDroneController : ControllerBase
{
    private static Drone _drone;
    private static DroneToDispatchGateway _gateway;
    private bool IsInitiatead = false;

    [HttpPost("InitDrone")]
    public async Task<InitDroneResponse> InitDrone(
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
    public async Task<AssignFleetResponse> AssignFleet(AssignFleetRequest assignFleetRequest)
    {
        Console.WriteLine($"SimDroneController.AssignFleet -> {assignFleetRequest.ToJson()}");
        Console.WriteLine($"{assignFleetRequest.DispatchUrl}");
        _gateway = new DroneToDispatchGateway(assignFleetRequest.DispatchUrl);
        _drone = new Drone(new DroneRecord
            {
                CurrentLocation = assignFleetRequest.HomeLocation,
                Destination = assignFleetRequest.HomeLocation,
                DispatchUrl = assignFleetRequest.DispatchUrl,
                DroneUrl = assignFleetRequest.DroneIp,
                HomeLocation = assignFleetRequest.HomeLocation,
                DroneId = assignFleetRequest.DroneId,
            }, 
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
    public async Task<AssignDeliveryResponse> AssignDelivery(
        AssignDeliveryRequest assignDeliveryRequest)
    {
        Console.WriteLine($"\n\n\nSimDroneController.AssignDelivery -> {assignDeliveryRequest.ToJson()}\n\n\n");
        return await _drone.DeliverOrder(assignDeliveryRequest);
    }


    [HttpPost("UpdateDroneStatus")]
    public async Task UpdateDroneStatus(UpdateDroneStatusRequest updateDroneStatusRequest)
    {
        await _gateway.UpdateDroneStatus(updateDroneStatusRequest);
    } 
}