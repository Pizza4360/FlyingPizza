using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using MongoDB.Bson;
using SimDrone.Controllers;
using static System.Decimal;

namespace SimDrone;

public class Drone : DroneRecord
{
    GeoLocation belmarPark = new() {Latitude = 39.70481995951833m, Longitude = -105.10536817563754m},
                msu = new() {Latitude = 39.74386695629378m, Longitude = -105.00610500179027m},
                aurora = new() {Latitude = 39.710573732539885m, Longitude = -104.81408307283085m},
                TractorSupplyCo = new() {Latitude = 40.20104629143965m, Longitude = -104.97856186942785m},
                LovesTravelStop = new() {Latitude = 40.30514631684113m, Longitude = -104.98423969309658m},
                VillageInnI25Hwy7 = new() {Latitude = 39.99955658640521m, Longitude = -104.97768379612273m};

    private SimDroneController _controller;


    public Drone(DroneRecord record, IBaseGateway<SimDroneController> gateway, SimDroneController controller)
    {
        DroneId = record.DroneId;
        HomeLocation = record.HomeLocation;
        BadgeNumber = record.BadgeNumber;
        DroneUrl = record.DroneUrl;
        _controller = controller;
        State = DroneState.Ready;
        CurrentLocation = HomeLocation;
        Destination = record.Destination;
    }


    public async Task<AssignDeliveryResponse> DeliverOrder(AssignDeliveryRequest request)
    {
        await UpdateStatus(DroneState.Delivering);
        await TravelTo(request.OrderLocation);
        Console.WriteLine("Done with delivery, returning home.");
        await UpdateStatus(DroneState.Returning);
        await TravelTo(HomeLocation);
        await UpdateStatus(DroneState.Ready);
        return new AssignDeliveryResponse
        {
            DroneId = DroneId,
            OrderId = OrderId,
            Success = true
        };
    }


    // Send an DroneState update to DispatcherGateway
    private async Task<UpdateDroneStatusResponse?> UpdateStatus(DroneState state)
    {
        State = state;
        return await PatchDroneStatus();
    }


    private async Task<UpdateDroneStatusResponse?> PatchDroneStatus()
    {
        var response = await _controller.UpdateDroneStatus(
            new UpdateDroneStatusRequest
            {
                DroneId = DroneId,
                State = State,
                Location = CurrentLocation
            });

        return response;
    }


    // Send an Location update to DispatcherGateway
    private void UpdateLocation(GeoLocation location)
    {
        CurrentLocation = location;
        PatchDroneStatus();
    }


    public override string ToString()
    {
        return $"SimDrone:{{DroneId:{DroneId},Location:{CurrentLocation},Destination:{Destination},State:{State}}}";
    }


    public async Task TravelTo(GeoLocation endingLocation)
    {
        Console.WriteLine($"Starting at {CurrentLocation.Latitude}");
        double DroneStepSizeInKilometers = .04; // .04 km == 40 meters every two seconds
        while(!CurrentLocation.Equals(endingLocation))
        {
            var bearing = Bearing(CurrentLocation.Latitude, CurrentLocation.Longitude, endingLocation.Latitude, endingLocation.Longitude);
            Console.WriteLine($"bearing between = {bearing}" );
            var newLocation = FindPointAtDistanceFrom(CurrentLocation, bearing , DroneStepSizeInKilometers);
            Console.WriteLine($"{newLocation.Latitude},{newLocation.Longitude}");
            UpdateLocation(newLocation);
            Thread.Sleep(2000);
        }
    }



    public static double ToRadians(double x) => x * Math.PI / 180;


    public static double ToDegrees(double x) => x * 180 / Math.PI;


    public static double Haversine(decimal alat, decimal alon, decimal blat, decimal blon)
    {
        double longitude = (double) alon, latitude = (double) alat, otherLongitude = (double) blon, otherLatitude = (double) blat;
        var d1 = latitude * (Math.PI / 180.0);
        var num1 = longitude * (Math.PI / 180.0);
        var d2 = otherLatitude * (Math.PI / 180.0);
        var num2 = otherLongitude * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }


    public static GeoLocation FindPointAtDistanceFrom(GeoLocation startLocation, double initialBearingInRadians, double distanceInKilometres)
    {
        const double radiusEarthKilometres = 6371.01;
        var distRatio = distanceInKilometres / radiusEarthKilometres;
        var distRatioSine = Math.Sin(distRatio);
        var distRatioCosine = Math.Cos(distRatio);

        var startLatRad = DegreesToRadians((double) startLocation.Latitude);
        var startLonRad = DegreesToRadians((double) startLocation.Longitude);

        var startLatCos = Math.Cos(startLatRad);
        var startLatSin = Math.Sin(startLatRad);

        var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingInRadians)));

        var endLonRads = startLonRad +
                         Math.Atan2(
                             Math.Sin(initialBearingInRadians) * distRatioSine * startLatCos,
                             distRatioCosine - startLatSin * Math.Sin(endLatRads));

        return new GeoLocation
        {
            Latitude = (decimal) RadiansToDegrees(endLatRads),
            Longitude = (decimal) RadiansToDegrees(endLonRads)
        };
    }


    public static double DegreesToRadians(double degrees)
    {
        const double degToRadFactor = Math.PI / 180;
        return degrees * degToRadFactor;
    }


    public static double RadiansToDegrees(double radians)
    {
        const double radToDegFactor = 180 / Math.PI;
        return radians * radToDegFactor;
    }


    public static double Bearing(decimal lat1Degrees, decimal lon1Degrees, decimal lat2Degrees, decimal lon2Degrees)
    {
        var startLat = ToRadians((double) lat1Degrees);
        var startLong = ToRadians((double) lon1Degrees);
        var endLat = ToRadians((double) lat2Degrees);
        var endLong = ToRadians((double) lon2Degrees);
        var dLong = endLong - startLong;
        var dPhi = Math.Log(Math.Tan(endLat / 2.0 + Math.PI / 4.0) / Math.Tan(startLat / 2.0 + Math.PI / 4.0));

        if(Math.Abs(dLong) > Math.PI)
        {
            dLong = (2.0 * Math.PI - dLong) * dLong > 0.0 ? -1 : 1;
        }

        return ToDegrees(Math.Atan2(dLong, dPhi) + 360.0) % 360;
    }
}

