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


    public async Task<AssignDeliveryResponse> AssignDelivery(AssignDeliveryRequest request)
    {
        await UpdateStatus(DroneState.Delivering);
        await Traverse(request.OrderLocation);
        Console.WriteLine("Done with delivery, returning home.");
        // await UpdateStatus(DroneState.Returning);
        // await Traverse(request.OrderLocation);
        // await UpdateStatus(DroneState.Ready);

        return new AssignDeliveryResponse
        {
            DroneId = DroneId,
            OrderId = OrderId,
            Success = true
        };
    }


    private async Task Traverse(GeoLocation destination)
    {
        Thread.Sleep(5000);
        Console.WriteLine($"Starting at {CurrentLocation.Latitude},{CurrentLocation.Longitude}");
        double DroneStepSizeInKilometers = .00004;
        while(!CurrentLocation.Equals(destination))
        {
            var bearing = Trig.Bearing(CurrentLocation.Latitude, CurrentLocation.Longitude, destination.Latitude, destination.Longitude);
            Console.WriteLine($"bearing between = {bearing}" );
            var newLocation = Trig.FindPointAtDistanceFrom(CurrentLocation, bearing , DroneStepSizeInKilometers);
            Console.WriteLine($"{newLocation.Latitude},{newLocation.Longitude}");
            CurrentLocation = newLocation;
            PatchDroneStatus();
            Thread.Sleep(2000);
        }
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


        public class Trig
    {
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

            var endLatRads = Math.Asin(startLatSin * distRatioCosine + startLatCos * distRatioSine * Math.Cos(initialBearingInRadians));

            var endLonRads = startLonRad + Math.Atan2( Math.Sin(initialBearingInRadians) * distRatioSine * startLatCos, distRatioCosine - startLatSin * Math.Sin(endLatRads));

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
            var startLat = ToRadians((double)lat1Degrees);
            var startLong = ToRadians((double)lon1Degrees);
            var endLat = ToRadians((double)lat2Degrees);
            var endLong = ToRadians((double)lon2Degrees);
            var dLong = endLong - startLong;
            var dPhi = Math.Log(Math.Tan(endLat / 2.0 + Math.PI / 4.0) / Math.Tan(startLat / 2.0 + Math.PI / 4.0));

            if(Math.Abs(dLong) > Math.PI)
            {
                dLong = (2.0 * Math.PI - dLong) * dLong > 0.0 ? -1 : 1;
            }

            return ToDegrees(Math.Atan2(dLong, dPhi) + 360.0) % 360;
        }
    }

}