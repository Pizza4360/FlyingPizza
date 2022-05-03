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
    private bool _isInitiated;

    [HttpPost("InitDrone")]
    public Task<InitDroneResponse> InitDrone(
        InitDroneRequest initDroneRequest)
    {
        var responseString = _isInitiated
            ? "This drone is already initialized"
            : "This drone is ready to be initialized to a fleet.";
        Console.WriteLine($"SimDroneController.InitDrone -> {initDroneRequest}\tresponse -> {responseString}");
        return Task.FromResult(new InitDroneResponse
        {
            DroneId = initDroneRequest.DroneId,
            Okay = !_isInitiated
        });
    }

    [HttpPost("RejoinFleet")]
    public async Task RejoinFleet(DroneRecoveryRequest request)
    {
        Console.WriteLine($"SimDroneController.RejoinFleet({request.Record})" +
                          $"\nflying home to {request.Record.HomeLocation}");
        await JoinFleet(request.Record);
        await _drone.TravelTo(request.Record.HomeLocation);
    }
    
    
    [HttpPost("HealthCheck")]
    public Task<DroneRecord> HealthCheck(PingDto s)
    {
        return Task.FromResult(new DroneRecord
        {
            DispatchUrl = _drone.DispatchUrl,
            CurrentLocation = _drone.CurrentLocation,
            Destination = _drone.Destination,
            DroneId = _drone.DroneId,
            DroneUrl = _drone.DroneUrl,
            HomeLocation = _drone.HomeLocation,
            OrderId = _drone.OrderId,
            State = _drone.State
        });
    }

    [NonAction]
    public async Task<bool> Recover(DroneRecord record)
    {
        Console.WriteLine($"\nSimDroneController.Recover...");
        if (string.IsNullOrEmpty(_gateway.EndPoint))
        {
            _gateway = new DroneToDispatchGateway(record.DispatchUrl);
        }
        return await _gateway.Recover(record);
    }
        
    [HttpPost("AssignFleet")]
    public async Task<AssignFleetResponse> AssignFleet(AssignFleetRequest request)
    {
        // Console.WriteLine($"{assignFleetRequest.DispatchUrl}");
        if (
            string.IsNullOrEmpty(request.DroneId) ||
            string.IsNullOrEmpty(request.DispatchUrl) ||
            string.IsNullOrEmpty(request.DroneUrl) ||
            request.HomeLocation == null)
        {
            throw new ArgumentException("DroneId, DispatchUrl, DroneUrl, and HomeLocation must all be valid to assign this drone to a fleet.");
        }
        var droneRecord = new DroneRecord
        {
            CurrentLocation = request.HomeLocation,
            Destination = request.HomeLocation,
            DispatchUrl = request.DispatchUrl,
            DroneUrl = request.DroneUrl,
            HomeLocation = request.HomeLocation,
            DroneId = request.DroneId,
            OrderId = null
        };
        await JoinFleet(droneRecord);
        Console.WriteLine($"\nSimDrone successfully initialized.\nDrone -->{_drone}");
        _isInitiated = true;
        await PersistRecord(droneRecord);
        return new AssignFleetResponse
        {
            FirstState = DroneState.Ready,
            DroneId = request.DroneId,
            IsInitializedAndAssigned = true
        };
    }

    [NonAction]
    private Task JoinFleet(DroneRecord record)
    {
        _drone = new Drone(record, _gateway, this);
        Console.WriteLine($"SimDroneController.JoinFleet, dispatch at {record.DispatchUrl}");
        _gateway = new DroneToDispatchGateway(record.DispatchUrl);
        return Task.CompletedTask;
    }

    [NonAction]
    private async Task PersistRecord(DroneRecord droneRecord)
    {
        Console.WriteLine("\nSaving drone drone record...");
        var file = !System.IO.File.Exists(DroneRecord.File()) 
            ? System.IO.File.Create(DroneRecord.File()) 
            : System.IO.File.Open(DroneRecord.File(), FileMode.Open);
        await using var writer = new StreamWriter(file);
        await writer.WriteAsync(droneRecord.ToJson());
        writer.Close();
        file.Close();
        Console.WriteLine("Done.");
    }


    [HttpPost("AssignDelivery")]
    public async Task<AssignDeliveryResponse> AssignDelivery(AssignDeliveryRequest assignDeliveryRequest)
    {
        return await _drone.DeliverOrder(assignDeliveryRequest);
    }


    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneRecord drone)
    {
        await PersistRecord(drone);
        return await _gateway.UpdateDroneStatus(drone.Update());
    }

    public async Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request)
    {
        return await _gateway.CompleteDelivery(request);
    }
}