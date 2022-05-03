using DecimalMath;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using MongoDB.Bson;
using SimDrone.Controllers;

namespace SimDrone;

public class Drone : DroneRecord
{
    private const decimal DroneStepSizeInKilometers = .0004m;
    private const decimal RadiusEarthKilometres = 6371.01m;
    private const decimal DegToRadFactor = DecimalEx.Pi / 180;
    private const decimal RadToDegFactor = 180 / DecimalEx.Pi;

    private readonly SimDroneController _controller;
    public Drone(DroneRecord record, IBaseGateway<SimDroneController> gateway, SimDroneController controller)
    {
        DroneId = record.DroneId;
        HomeLocation = record.HomeLocation;
        DroneUrl = record.DroneUrl;
        DispatchUrl = record.DispatchUrl;
        _controller = controller;
        State = DroneState.Ready;
        CurrentLocation = HomeLocation;
        Destination = record.Destination;
    }

    public override string ToString()
    {
        return this.ToJson();
    }

    private static decimal ToRadians(decimal x)
    {
        return x * DecimalEx.Pi / 180;
    }

    private static decimal ToDegrees(decimal x)
    {
        return x * 180 / DecimalEx.Pi;
    }

    private static decimal DegreesToRadians(decimal degrees)
    {
        return degrees * DegToRadFactor;
    }

    private static decimal RadiansToDegrees(decimal radians)
    {
        return radians * RadToDegFactor;
    }

    private static decimal Abs(decimal x)
    {
        return x > 0 ? x : -x;
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
    
    private async Task<UpdateDroneStatusResponse?> UpdateStatus(DroneState state)
    {
        if (State == DroneState.Delivering && state == DroneState.Returning)
        {
            Destination = HomeLocation;
            OrderId = null;
        }

        State = state;
        return await PatchDroneStatus();
    }

    private async Task<UpdateDroneStatusResponse?> UpdateOrderId(DroneState state)
    {
        if (State == DroneState.Delivering && state == DroneState.Returning) Destination = HomeLocation;
        State = state;
        return await PatchDroneStatus();
    }

    // OrderState ??

    private static decimal HaversineInMeters(GeoLocation locationA, GeoLocation locationB)
    {
        decimal longitude = locationA.Longitude,
            latitude = locationA.Latitude,
            otherLongitude = locationB.Longitude,
            otherLatitude = locationB.Latitude;
        var d1 = latitude * (DecimalEx.Pi / 180.0m);
        var num1 = longitude * (DecimalEx.Pi / 180.0m);
        var d2 = otherLatitude * (DecimalEx.Pi / 180.0m);
        var num2 = otherLongitude * (DecimalEx.Pi / 180.0m) - num1;
        var d3 = DecimalEx.Pow(DecimalEx.Sin((d2 - d1) / 2.0m), 2.0m) + DecimalEx.Cos(d1) * DecimalEx.Cos(d2) *
            DecimalEx.Pow(DecimalEx.Sin(num2 / 2.0m), 2.0m);
        return RadiusEarthKilometres * 2.0m * DecimalEx.ATan2(DecimalEx.Sqrt(d3), DecimalEx.Sqrt(1.0m - d3));
    }

    private static GeoLocation GetNextLocation(GeoLocation startLocation, decimal initialBearingInRadians,
        decimal distanceInKilometres)
    {
        var distRatio = distanceInKilometres / RadiusEarthKilometres;
        var distRatioSine = DecimalEx.Sin(distRatio);
        var distRatioCosine = DecimalEx.Cos(distRatio);

        var startLatRad = DegreesToRadians(startLocation.Latitude);
        var startLonRad = DegreesToRadians(startLocation.Longitude);

        var startLatCos = DecimalEx.Cos(startLatRad);
        var startLatSin = DecimalEx.Sin(startLatRad);

        var endLatRads = DecimalEx.ASin(startLatSin * distRatioCosine +
                                        startLatCos * distRatioSine * DecimalEx.Cos(initialBearingInRadians));

        var endLonRads = startLonRad +
                         DecimalEx.ATan2(
                             DecimalEx.Sin(initialBearingInRadians) * distRatioSine * startLatCos,
                             distRatioCosine - startLatSin * DecimalEx.Sin(endLatRads));

        return new GeoLocation
        {
            Latitude = RadiansToDegrees(endLatRads),
            Longitude = RadiansToDegrees(endLonRads)
        };
    }

    private static decimal Bearing(decimal lat1Degrees, decimal lon1Degrees, decimal lat2Degrees, decimal lon2Degrees)
    {
        var startLat = ToRadians(lat1Degrees);
        var startLong = ToRadians(lon1Degrees);
        var endLat = ToRadians(lat2Degrees);
        var endLong = ToRadians(lon2Degrees);
        var dLong = endLong - startLong;
        var dPhi = DecimalEx.Log(DecimalEx.Tan(endLat / 2.0m + DecimalEx.Pi / 4.0m) /
                                 DecimalEx.Tan(startLat / 2.0m + DecimalEx.Pi / 4.0m));

        if (Abs(dLong) > DecimalEx.Pi) dLong = (2.0m * DecimalEx.Pi - dLong) * dLong > 0.0m ? -1 : 1;

        return ToDegrees(DecimalEx.ATan2(dLong, dPhi) + 360.0m) % 360;
    }

    public async Task TravelTo(GeoLocation endingLocation)
    {
        Console.WriteLine($"Starting at {CurrentLocation.Latitude}");
        var buffer = new GeoLocation[5];
        buffer[0] = CurrentLocation;
        for (var i = 0; !CurrentLocation.Equals(endingLocation); i++)
        {
            Console.WriteLine($"{HaversineInMeters(CurrentLocation, endingLocation)} meters away");
            var bearing = Bearing(CurrentLocation.Latitude, CurrentLocation.Longitude, endingLocation.Latitude,
                endingLocation.Longitude);
            Console.WriteLine($"bearing between = {bearing}");
            buffer[i % 5] = CurrentLocation = GetNextLocation(CurrentLocation, bearing, DroneStepSizeInKilometers);
            Console.WriteLine($"{CurrentLocation}");
            if (i % 100 != 0) continue;
            UpdateLocation(CurrentLocation);
            Thread.Sleep(500);
        }
    }

    public async Task<AssignDeliveryResponse> DeliverOrder(AssignDeliveryRequest request)
    {
        OrderId = request.OrderId;
        await UpdateStatus(DroneState.Delivering);
        await TravelTo(request.OrderLocation);
        _controller.CompleteDelivery(
            new CompleteOrderRequest
            {
                DroneId = DroneId,
                OrderId = OrderId,
                Time = DateTime.Now
            });
        Console.WriteLine("Done with delivery, returning home.");
        await UpdateStatus(DroneState.Returning);
        await TravelTo(HomeLocation);
        await UpdateStatus(DroneState.Ready);
        OrderId = request.OrderId;
        return new AssignDeliveryResponse
        {
            DroneId = DroneId,
            OrderId = OrderId,
            Success = true
        };
    }
}