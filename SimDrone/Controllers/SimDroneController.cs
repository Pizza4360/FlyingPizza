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
    private bool IsInitiatead;
    private string RecordFile = "DroneRecord.json";

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

    [HttpPost("HealthCheck")]
    public async Task<DroneRecord> HealthCheck(string s)
    {
        return new DroneRecord
        {
            DispatchUrl = _drone.DispatchUrl,
            CurrentLocation = _drone.CurrentLocation,
            Destination = _drone.Destination,
            DroneId = _drone.DroneId,
            DroneUrl = _drone.DroneUrl,
            HomeLocation = _drone.HomeLocation,
            OrderId = _drone.OrderId,
            State = _drone.State
        };
    }

    [HttpPost("Revive")]
    public async Task<bool> Revive(DroneRecord record)
    {
        Console.WriteLine($"\n\n\nSimDroneController.Revive...");
        if (_gateway == null || string.IsNullOrEmpty(_gateway.EndPoint))
        {
            _gateway = new DroneToDispatchGateway(record.DispatchUrl);
        }

        return await _gateway.Revive(record);
    }
        
    [HttpPost("AssignFleet")]
    public async Task<AssignFleetResponse> AssignFleet(AssignFleetRequest assignFleetRequest)
    {
        Console.WriteLine($"\n\n\nSimDroneController.AssignFleet -> " +
                          $"CurrentLocation = {assignFleetRequest.HomeLocation}," +
                          $"Destination = {assignFleetRequest.HomeLocation}," +
                          $"DispatchUrl = {assignFleetRequest.DispatchUrl}," +
                          $"DroneUrl = {assignFleetRequest.DroneIp}," +
                          $"HomeLocation = {assignFleetRequest.HomeLocation}," +
                          $"{assignFleetRequest.ToJson()}");
        // Console.WriteLine($"{assignFleetRequest.DispatchUrl}");
        _gateway = new DroneToDispatchGateway(assignFleetRequest.DispatchUrl);
        var droneRecord = new DroneRecord
        {
            CurrentLocation = assignFleetRequest.HomeLocation,
            Destination = assignFleetRequest.HomeLocation,
            DispatchUrl = assignFleetRequest.DispatchUrl,
            DroneUrl = assignFleetRequest.DroneIp,
            HomeLocation = assignFleetRequest.HomeLocation,
            DroneId = assignFleetRequest.DroneId,
            OrderId = null
        };
        _drone = new Drone(droneRecord,
            this);
        Console.WriteLine($"\n\n\nSimDrone successfully initialized.\nDrone -->{_drone}");
        IsInitiatead = true;
        //todo write current drone record to file
        await PersistRecord(droneRecord);
        return new AssignFleetResponse
        {
            FirstState = DroneState.Ready,
            DroneId = assignFleetRequest.DroneId,
            IsInitializedAndAssigned = true
        };
    }

    [NonAction]
    private async Task PersistRecord(DroneRecord droneRecord)
    {
        Console.WriteLine("\n\n\nSaving drone state...");
        RecordFile = "test.txt";
        if (!System.IO.File.Exists(RecordFile))
        {
            System.IO.File.Create(RecordFile);
        }
        await using var writer = new StreamWriter(RecordFile, false);
        await writer.WriteAsync(_drone.ToJson());
        writer.Close();
    }


    [HttpPost("AssignDelivery")]
    public async Task<AssignDeliveryResponse> AssignDelivery(
        AssignDeliveryRequest assignDeliveryRequest)
    {
        Console.WriteLine($"SimDroneController.AssignDelivery -> {assignDeliveryRequest}");
        return await _drone.DeliverOrder(assignDeliveryRequest);
    }


    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneRecord updateDroneStatusRequest)
    {
        PersistRecord(updateDroneStatusRequest);
        return await _gateway.UpdateDroneStatus(updateDroneStatusRequest.Update());
    }

    public async Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request)
    {
        return await _gateway.CompleteDelivery(request);
    }
}