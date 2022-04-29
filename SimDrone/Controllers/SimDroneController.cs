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
        Console.WriteLine($"{assignFleetRequest.DispatchIp}");
        _gateway = new DroneToDispatchGateway(assignFleetRequest.DispatchIp);
        _drone = new Drone(new DroneRecord
            {
                BadgeNumber = assignFleetRequest.BadgeNumber,
                CurrentLocation = assignFleetRequest.HomeLocation,
                Destination = assignFleetRequest.HomeLocation,
                DispatchUrl = assignFleetRequest.DispatchIp,
                DroneUrl = assignFleetRequest.DroneIp,
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
    public async Task<AssignDeliveryResponse> AssignDelivery(
        AssignDeliveryRequest assignDeliveryRequest)
    {
        Console.WriteLine($"SimDroneController.AssignDelivery -> {assignDeliveryRequest}");
        return await _drone.DeliverOrder(assignDeliveryRequest);
    }


    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(UpdateDroneStatusRequest updateDroneStatusRequest)
        => await _gateway.UpdateDroneStatus(updateDroneStatusRequest);

    public async Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest completeOrderRequest)
    {
        return await _gateway.CompleteOrder(completeOrderRequest);
    }
}