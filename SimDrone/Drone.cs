using Domain;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
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
    private DroneToDispatchGateway DroneToDispatchGateway { get; set; }

    public Drone(DroneRecord record, DroneToDispatchGateway gateway)
    {
        Id = record.Id;
        HomeLocation = record.HomeLocation;
        BadgeNumber = record.BadgeNumber;
        DroneIp = record.DroneIp;
        DroneToDispatchGateway = gateway;
        State = DroneState.Ready;
        CurrentLocation = HomeLocation;
        Destination = record.Destination;
    }

    /// <summary>
    ///  Return an array of Geolocations representing a drone's delivery route.
    /// </summary>
    /// <returns>The GeoLocations which will simulate drone movement over time.</returns>
    /// <exception cref="ArgumentException"></exception>
    public GeoLocation[] GetRoute()
    {
        if (HomeLocation.Equals(Destination))
            throw new ArgumentException(
                "Destination cannot be the same as the AssignDeliveryRequest station!");

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
        var route = new List<GeoLocation>();
        decimal latDirection = Destination.Latitude - Destination.Longitude > 0 ? 1 : -1;
        decimal lonDirection = Destination.Latitude - Destination.Longitude > 0 ? 1 : -1;
        var latStep = latMax / (decimal)haversine * StepSize;
        var lonStep = lonMax / (decimal)haversine * StepSize;
        var latSum = 0m;
        var lonSum = 0m;
        while (latSum > latMax ||
               lonSum < lonMax)
        {
            latSum += lonDirection / latStep;
            lonSum += lonDirection / lonStep;
            route.Add(new GeoLocation
            {
                Latitude = latSum,
                Longitude = lonSum
            });
        }
        return route.ToArray();
    }
    

    // Tell SimDrone to deliver an order
    public bool DeliverOrder(GeoLocation customerLocation)
    {
        Destination = customerLocation;
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
        return true;
    }
    

    // Send an DroneState update to DispatcherGateway
    private bool UpdateStatus(DroneState state)
    {
        State = state;
        return PatchDroneStatus();
    }

    private bool PatchDroneStatus()
    {
        var t = DroneToDispatchGateway.PatchDroneStatus(
            new DroneStatusUpdateRequest
            {
                Id = Id,
                State = State
            });
        t.Wait();
        return t.IsCompletedSuccessfully;
    }
    
    // Send an Location update to DispatcherGateway
    private void UpdateLocation(GeoLocation location)
    {
        CurrentLocation = location;
        PatchDroneStatus();
    }

    public override string ToString()
    {
        return $"SimDrone:{{DroneId:{Id},Location:{CurrentLocation},Destination:{Destination},State:{State}}}";
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