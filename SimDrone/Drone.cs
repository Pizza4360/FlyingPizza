using DecimalMath;
using Domain;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using MongoDB.Bson;
using SimDrone.Controllers;


namespace SimDrone;

public class Drone : DroneEntity
{
    private const decimal DroneStepSizeInKilometers = .0004m;

    private SimDroneController _controller;
    public Drone(DroneEntity entity, IDroneToDispatchGateway gateway, SimDroneController controller)
    {
        DroneId = entity.DroneId;
        HomeLocation = entity.HomeLocation;
        DroneUrl = entity.DroneUrl;
        _controller = controller;
        LatestStatus = DroneStatus.Ready;
        CurrentLocation = HomeLocation;
        Destination = entity.Destination;
        BearingInDegrees = 0m;
    }

    public override string ToString()
    {
        return this.ToJson();
    }

    private async Task<UpdateDroneStatusResponse?> PatchDroneStatus()
    {
        var response = await _controller.UpdateDroneStatus(this);
        return response;
    }

    private void UpdateLocation(GeoLocation location)
    {
        CurrentLocation = location;
        PatchDroneStatus();
    }

    private async Task<UpdateDroneStatusResponse?> UpdateStatus(DroneStatus status)
    {
        if (LatestStatus == DroneStatus.Delivering && status == DroneStatus.Returning)
        {
            Destination = HomeLocation;
            DeliveryId = null;
        }

        LatestStatus = status;
        return await PatchDroneStatus();
    }

    private async Task<UpdateDroneStatusResponse?> UpdateDeliveryId(DroneStatus status)
    {
        if (LatestStatus == DroneStatus.Delivering && status == DroneStatus.Returning) Destination = HomeLocation;
        LatestStatus = status;
        return await PatchDroneStatus();
    }
    
    public async Task TravelTo(GeoLocation endingLocation)
    {
        Destination = endingLocation;
        Console.WriteLine($"Starting at {CurrentLocation.Latitude}");
        var buffer = new GeoLocation[5];
        buffer[0] = CurrentLocation;
        for (var i = 0; !CurrentLocation.Equals(endingLocation); i++)
        {
            Console.WriteLine($"{GeoCalculations.HaversineInMeters(CurrentLocation, endingLocation)} meters away");
            BearingInDegrees = GeoCalculations.Bearing(CurrentLocation.Latitude, CurrentLocation.Longitude, endingLocation.Latitude,
                endingLocation.Longitude);
            Console.WriteLine($"bearing between = {BearingInDegrees}");
            buffer[i % 5] = CurrentLocation = GeoCalculations.GetNextLocation(CurrentLocation, BearingInDegrees, DroneStepSizeInKilometers);
            Console.WriteLine($"{CurrentLocation}");
            if (i % 100 != 0) continue;
            UpdateLocation(CurrentLocation);
            Thread.Sleep(500);
        }
    }
    public async Task<AssignDeliveryResponse> DeliverDelivery(AssignDeliveryRequest request)
    {
        DeliveryId = request.DeliveryId;
        await UpdateStatus(DroneStatus.Delivering);
        await TravelTo(request.DeliveryLocation);
        _controller.CompleteDelivery(
            new CompleteDeliveryRequest
            {
                DroneId = DroneId,
                DeliveryId = DeliveryId,
                Time = DateTime.Now
            });
        Console.WriteLine("Done with delivery, returning home.");
        await UpdateStatus(DroneStatus.Returning);
        await TravelTo(HomeLocation);
        await UpdateStatus(DroneStatus.Ready);
        DeliveryId = request.DeliveryId;
        return new AssignDeliveryResponse
        {
            DroneId = DroneId,
            DeliveryId = DeliveryId,
            Success = true
        };
    }
    public void ChangeController(SimDroneController sim)
    {
        _controller = sim;
    }
}