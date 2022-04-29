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

    [HttpPost("HealthCheck")]
    public async Task<DroneRecord> HealthCheck(string s)
    {
        return new DroneRecord
        {
            DispatchUrl = _drone.DispatchUrl,
            BadgeNumber = _drone.BadgeNumber,
            CurrentLocation = _drone.CurrentLocation,
            Destination = _drone.Destination,
            DroneId = _drone.DroneId,
            DroneUrl = _drone.DroneUrl,
            HomeLocation = _drone.HomeLocation,
            OrderId = _drone.OrderId,
            State = _drone.State
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
    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneUpdate updateDroneStatusRequest)
        => await _gateway.UpdateDroneStatus(updateDroneStatusRequest);

    public async Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request)
    {
        return await _gateway.CompleteDelivery(request);
    }
}