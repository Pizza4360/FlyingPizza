using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using SimDrone.Controllers;
using static System.Decimal;

namespace SimDrone;

public class Drone : DroneRecord
{
    /// Radius of the Earth used in calculating distance 
    private const int EarthRadius = 6371;

    /// 20 MPH as meters per second
    private const double DroneSpeed = 0.0089408;

    /// Number of milliseconds to wait before updating SimDrone status
    private const int DroneUpdateInterval = 2000;

    /// I don't think this makes sense but it's working...
    private const decimal StepSize = DroneUpdateInterval / 1000.0m * (decimal) DroneSpeed;

    /// Allows the simulation to communicate with the dispatcher.
    private IBaseGateway<SimDroneController> DroneToDispatchGateway { get; set; }

    private SimDroneController _controller;

    public Drone(DroneRecord record, IBaseGateway<SimDroneController> gateway, SimDroneController controller)
    {
        DroneId = record.DroneId;
        HomeLocation = record.HomeLocation;
        BadgeNumber = record.BadgeNumber;
        DroneIp = record.DroneIp;
        DroneToDispatchGateway = gateway;
        _controller = controller;
        State = DroneState.Ready;
        CurrentLocation = HomeLocation;
        Destination = record.Destination;
    }


    public GeoLocation[] GetRoute()
    {
        if (HomeLocation.Equals(Destination))
            throw new ArgumentException(
                "Destination cannot be the same as the AssignDelivery station!");

        var haversine = Haversine(
            ToDouble(HomeLocation.Latitude), 
            ToDouble(HomeLocation.Longitude),
            ToDouble(Destination.Latitude), 
            ToDouble(Destination.Longitude)
            );

        var latMax  = (decimal)Haversine(
                ToDouble(HomeLocation.Latitude), 
                ToDouble(HomeLocation.Longitude),
                ToDouble(Destination.Latitude), 
                ToDouble(HomeLocation.Longitude)
            );
        var lonMax = (decimal)Haversine(
                ToDouble(HomeLocation.Latitude), 
                ToDouble(HomeLocation.Longitude),
                ToDouble(HomeLocation.Latitude), 
                ToDouble(Destination.Longitude)
            );
        // TODO: direction is determined incorrectly
        // TODO: You may want behavior:
        // TODO: latDirection = Destination.Latitude - HomeLocation.Lattitude < 0 ? 1 : -1
        // TODO: longDirection = Destination.Longitude - HomeLocation.Longitude < 0 ? 1 : -1

        var route = new List<GeoLocation>();
        decimal latDirection = Destination.Latitude - Destination.Longitude > 0 ? 1 : -1;
        decimal lonDirection = Destination.Latitude - Destination.Longitude > 0 ? 1 : -1;
        // TODO: latMax/stepSize is the behavior you want, but rolling a for loop will do better
        var latStep = latMax / (decimal)haversine * StepSize;
        var lonStep = lonMax / (decimal)haversine * StepSize;
        var latSum = 0m;
        var lonSum = 0m;
        while (latSum > latMax ||
               lonSum < lonMax)
        {
            latSum += latDirection / latStep;
            lonSum += lonDirection / lonStep;
            // TODO: incorrect use of haversine, converting kilometers to Arc Distance Geolocation
            // TODO: options: discard haversine or make inverseHaversine to make Geolocation
            route.Add(new GeoLocation
            {
                Latitude = latSum,
                Longitude = lonSum
            });
        }
        return route.ToArray();
    }
    

    // Tell SimDrone to deliver an order
    public AssignDeliveryResponse AssignDelivery(AssignDeliveryRequest request)
    {
        var assignDeliveryResponse = new AssignDeliveryResponse
        {
            OrderId = request.OrderId,
            DroneId = DroneId,
            Success = false
        };
        Destination = request.OrderLocation;
        UpdateStatus(DroneState.Delivering);
        var route = GetRoute();
        var s = string.Join(",", route.Select(x => $"{{{x.Latitude},{x.Longitude}}}").ToArray());
        Console.WriteLine($"{this},route:{s}"); // Debug

        foreach (var location in route)
        {
            UpdateLocation(location);
            Console.WriteLine(this); // Debug
            Thread.Sleep(DroneUpdateInterval);
        }

        Console.WriteLine("Order Delivered! Returning to Home...");
        UpdateStatus(DroneState.Returning);
        foreach (var location in route.Reverse())
        {
            UpdateLocation(location);
            Console.WriteLine(this); // Debug
            Thread.Sleep(DroneUpdateInterval);
        }

        UpdateStatus(DroneState.Ready);
        Console.WriteLine("Back home!"); // Debug

        assignDeliveryResponse.Success = true;
        return assignDeliveryResponse;
    }
    

    // Send an DroneState update to DispatcherGateway
    private UpdateDroneStatusResponse? UpdateStatus(DroneState state)
    {
        State = state;
        return PatchDroneStatus();
    }

    private UpdateDroneStatusResponse? PatchDroneStatus()
    {
        var response = _controller.UpdateDroneStatus(
            new UpdateDroneStatusRequest
            {
                DroneId = DroneId,
                State = State
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

    // Helper function for Haversine formula readability
    private static double ToRadians(double x)
    {
        return Math.PI / 180 * x;
    }

    // Helper function for Haversine formula readability
    private static double SinSquared(double x)
    {
        return Math.Pow(Math.Sin(ToRadians(x) / 2), 2);
    }

    // Calculate the difference between two points on a sphere
    private static double Haversine(
        double lat1, double lon1,
        double lat2, double lon2)
    {
        return EarthRadius * 2 * Math.Asin(
            Math.Sqrt(SinSquared(lat2 - lat1)) +
            SinSquared((lon2 - lon1) * Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))));
    }
}