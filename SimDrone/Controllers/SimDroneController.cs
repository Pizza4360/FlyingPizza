using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace SimDrone.Controllers;

[ApiController]
[Route("[controller]")]
public class SimDroneController : ControllerBase
{
    private static Drone _drone;
    private static IDroneToDispatchGateway _gateway;
    private bool IsInitiatead;

    [HttpPost("InitDrone")]
    public async Task<InitDroneResponse> InitDrone(
        InitDroneRequest initDroneRequest)
    {
        var responseString = IsInitiatead
            ? "This drone is already initialized"
            : "This drone is ready to be initialized to a fleet.";
        Console.WriteLine($"SimDroneController.InitDrone -> {initDroneRequest}\tresponse -> {responseString}");
        return new InitDroneResponse
        {
            DroneId = initDroneRequest.DroneId,
            Okay = !IsInitiatead
        };
    }

    [HttpPost("RejoinFleet")]
    public async Task RejoinFleet(RecoveryRequest request)
    {
        Console.WriteLine($"SimDroneController.RejoinFleet({request.Entity})");
        await JoinFleet(request.Entity);
        await _drone.TravelTo(request.Entity.HomeLocation);
    }
    
    
    [HttpPost("HealthCheck")]
    public async Task<DroneEntity> HealthCheck(BaseDto s)
    {
        return new DroneEntity
        {
            DispatchUrl = _drone.DispatchUrl,
            CurrentLocation = _drone.CurrentLocation,
            Destination = _drone.Destination,
            DroneId = _drone.DroneId,
            DroneUrl = _drone.DroneUrl,
            HomeLocation = _drone.HomeLocation,
            DeliveryId = _drone.DeliveryId,
            LatestStatus = _drone.LatestStatus
        };
    }

    [NonAction]
    public async Task<bool> Revive(DroneEntity entity)
    {
        Console.WriteLine($"\nSimDroneController.Revive...");
        if (_gateway == null || string.IsNullOrEmpty(_gateway.GetEndPoint()))
        {
            _gateway = new DroneToDispatchGateway(entity.DispatchUrl);
        }
        return await _gateway.Revive(entity);
    }
        
    [HttpPost("AssignFleet")]
    public async Task<AssignFleetResponse> AssignFleet(AssignFleetRequest assignFleetRequest)
    {
        Console.WriteLine($"AssignFleet -------------> {assignFleetRequest.DispatchUrl}");
        var droneModel = new DroneEntity
        {
            CurrentLocation = assignFleetRequest.HomeLocation,
            Destination = assignFleetRequest.HomeLocation,
            DispatchUrl = assignFleetRequest.DispatchUrl,
            DroneUrl = assignFleetRequest.DroneUrl,
            HomeLocation = assignFleetRequest.HomeLocation,
            DroneId = assignFleetRequest.DroneId,
            DeliveryId = ""
        };
        await JoinFleet(droneModel);
        Console.WriteLine($"\nSimDrone successfully initialized.\nDrone -->{_drone}");
        IsInitiatead = true;
        //todo write current drone model to file
        await PersistModel(droneModel);
        return new AssignFleetResponse
        {
            FirstStatus = DroneStatus.Ready,
            DroneId = assignFleetRequest.DroneId,
            IsInitializedAndAssigned = true
        };
    }

    [NonAction]
    private Task JoinFleet(DroneEntity entity)
    {
        _drone = new Drone(entity, _gateway, this);
        _gateway = new DroneToDispatchGateway(entity.DispatchUrl);
        return Task.CompletedTask;
    }

    [NonAction]
    private async Task PersistModel(DroneEntity droneEntity)
    {
        _drone.DispatchUrl ??= droneEntity.DispatchUrl;
        _drone.DispatchUrl = droneEntity.DispatchUrl;
        Console.WriteLine("\nSaving drone status...");

        FileStream file;
        if (!System.IO.File.Exists(DroneEntity.File()))
        {
            file = System.IO.File.Create(DroneEntity.File());
        }
        else
        {
            file = System.IO.File.Open(DroneEntity.File(), FileMode.Open);
        }
        await using var writer = new StreamWriter(file);
        await writer.WriteAsync(_drone.ToJson());
        writer.Close();
        file.Close();
    }


    [HttpPost("AssignDelivery")]
    public async Task<AssignDeliveryResponse> AssignDelivery(AssignDeliveryRequest assignDeliveryRequest)
    {
        return await _drone.DeliverDelivery(assignDeliveryRequest);
    }


    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneEntity updateDroneStatusRequest)
    {
        PersistModel(updateDroneStatusRequest);
        return await _gateway.UpdateDroneStatus(updateDroneStatusRequest.Update());
    }

    public async Task<CompleteDeliveryResponse> CompleteDelivery(CompleteDeliveryRequest request)
    {
        return await _gateway.CompleteDelivery(request);
    }
    public void ChangeGateway(IDroneToDispatchGateway mockGatewayObject)
    {
        _gateway = mockGatewayObject;
    }
    public void ChangeDrone(Drone newDrone)
    {
        _drone = newDrone;
    }
}