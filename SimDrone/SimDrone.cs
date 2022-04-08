using Domain;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.Entities;
using Domain.Implementation.Gateways;
using Domain.Interfaces.Gateways;
using static System.Decimal;

namespace SimDrone;

public class Drone : DroneRecord
{
    // Radius of the Earth used in calculating distance 
    private const int EarthRadius = 6371;

    // 20 MPH as meters per second
    private const double DroneSpeed = 0.0089408;

    // Number of milliseconds to wait before updating SimDrone status
    private const int DroneUpdateInterval = 2000;

    // I don't think this makes sense but it's working...
    private const decimal StepSize = DroneUpdateInterval / 1000.0m * (decimal) DroneSpeed;

    private readonly DroneToDispatcherGateway _droneToDispatcher;

    public Drone(string id, GeoLocation homeLocation, DroneToDispatcherGateway gateway, int badgeNumber, string ipAddress,
        string url)
    {
        Id = id;
        HomeLocation = homeLocation;
        _droneToDispatcher = gateway;
        CurrentLocation = HomeLocation;
        Destination = HomeLocation;
        BadgeNumber = badgeNumber;
        State = DroneState.Ready;
        IpAddress = ipAddress;
        DispatcherUrl = url;
    }

    // Return an array of Geolocations representing a drone's delivery route
    public GeoLocation[] GetRoute()
    {
        if (HomeLocation.Equals(Destination))
            throw new ArgumentException(
                "Destination cannot be the same as the Delivery station!");

        var distance = Haversine(ToDouble(HomeLocation.Latitude), ToDouble(HomeLocation.Longitude),
            ToDouble(Destination.Latitude), ToDouble(Destination.Longitude));

        var numberOfLocations = (int) Math.Floor((decimal) distance / StepSize);

        Console.WriteLine($"need to travel {distance} km, step_size={StepSize}, num locations={numberOfLocations}");
        // Latitude distance to get to destination
        var xStep = (Destination.Longitude - HomeLocation.Longitude) / numberOfLocations;

        // Longitude distance to get to destination
        var yStep = (Destination.Latitude - HomeLocation.Latitude) / numberOfLocations;

        // LINQ yields all the points (except possibly the last one) along the route, one unit apart.
        var route = Enumerable.Range(0, numberOfLocations - 1)
            .Select(i => new GeoLocation
            {
                Latitude = i * xStep,
                Longitude = i * yStep
            }).ToList();

        // Add the Destination Point if needed.
        if (!route.Last().Equals(Destination)) route.Add(Destination);

        return route.ToArray();
    }

    // Tell SimDrone to deliver an order
    public void DeliverOrder(GeoLocation customerLocation)
    {
        Destination = customerLocation;
        UpdateDroneState(DroneState.Delivering);
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
        UpdateDroneState(DroneState.Returning);
        foreach (var location in route.Reverse())
        {
            UpdateLocation(location);
            Console.WriteLine(this); // Debug
            Thread.Sleep(DroneUpdateInterval);
        }

        UpdateDroneState(DroneState.Ready);
        Console.WriteLine("Back home!"); // Debug
    }

    // Send an DroneState update to DispatcherGateway
    private void UpdateDroneState(string state)
    {
        State = state;
        _droneToDispatcher.PutDroneState(
            new UpdateStatusDto
            {
                Id = Id,
                State = $"{state}"
            });
    }

    // Send an Location update to DispatcherGateway
    private void UpdateLocation(GeoLocation location)
    {
        CurrentLocation = location;
        _droneToDispatcher.PutDroneState(
            new UpdateStatusDto
            {
                Id = Id,
                Location = CurrentLocation
            });
    }

    public override string ToString()
    {
        return $"SimDrone:{{Id:{Id},Location:{CurrentLocation},Destination:{Destination},State:{State}}}";
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