using System;
using System.Threading;
using System.Linq;
using Domain.Interfaces.Gateways;
using Domain.Entities;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using static System.Decimal;

namespace DroneSimulator
{
    public enum DroneState
    {
        Ready,
        Delivering,
        Returning,
        Dead,
        Charging
    }

    public class Drone
    {
        // Radius of the Earth used in calculating distance 
        private const int EarthRadius = 6371;

        // 20 MPH as meters per second
        private const double DroneSpeed = 0.0089408;
        
        // Number of milliseconds to wait before updating Drone status
        private const int DroneUpdateInterval = 2000;

        // I don't think this makes sense but it's working...
        private const double StepSize = DroneUpdateInterval / 1000 * DroneSpeed;
        
        // The unique ID of this drone which is stored in the database
        private string Id { get; }

        // The point representing the pizza restaurant
        private GeoLocation Home { get; }

        // The current position of the drone
        private GeoLocation Location { get; set; }

        // The desired position of the drone
        private GeoLocation Destination { get; set; }

        // Current state of the drone
        private DroneState State { get; set; }

        // Gateway for communication with the dispatcher
        private readonly IDispatcherGateway _dispatcher;

        // Constructor
        public Drone(string id, GeoLocation home, IDispatcherGateway dispatcher)
        {
            Id = id;
            Location = home;
            Destination = home;
            State = DroneState.Ready;
            _dispatcher = dispatcher;
            Home = home;
        }
        
        // Return an array of Point records simulating a drone's delivery route
        private GeoLocation[] GetRoute()
        {
            if (Home.Equals(Destination))
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            var distance = Haversine(ToDouble(Home.Latitude), ToDouble(Home.Longitude),
                ToDouble(Destination.Latitude), ToDouble(Destination.Longitude));

            var numberOfLocations = (int)Math.Floor(distance / StepSize);

            Console.WriteLine($"need to travel {distance} km, step_size={StepSize}, num locations={numberOfLocations}");
            // Latitude distance to get to destination
            var xStep = (Home.Longitude - Destination.Longitude) / numberOfLocations;
            
            // Longitude distance to get to destination
            var yStep = (Home.Latitude - Destination.Latitude) / numberOfLocations;
            
            // LINQ yields all the points (except possibly the last one) along the route, one unit apart.
            var route = Enumerable.Range(0, numberOfLocations - 1)
                .Select(i => new GeoLocation
                {
                    Latitude = i * xStep,
                    Longitude = i * yStep
                }).ToList();

            // Add the Destination Point if needed.
            if (!route.Last().Equals(Destination))
            {
                route.Add(Destination);
            }

            return route.ToArray();
        }

        // Dispatch a drone to deliver a pizza.
        public void DeliverOrder(GeoLocation customerLocation)
        {
            Destination = customerLocation;
            UpdateStatus(DroneState.Delivering);
            var route = GetRoute();
            var s = string.Join(",", route.Select(x => $"{{{x.Latitude},{x.Longitude}}}").ToArray());
            Console.WriteLine($"{this},route:{s}"); // Debug

            foreach (var location in route)
            {
                Location = location;
                UpdateLocation();
                Console.WriteLine(this); // Debug
                Thread.Sleep(DroneUpdateInterval);
            }

            Console.WriteLine("Order complete!");
            UpdateStatus(DroneState.Returning);
            for (int i = route.Length - 1; i > 0; i--)
            {
                Location = route[i];
                UpdateLocation();
                Console.WriteLine(this); // Debug
                Thread.Sleep(DroneUpdateInterval);
            }

            UpdateStatus(DroneState.Ready);
            Console.WriteLine("Back home!"); // Debug
        }

        private void UpdateStatus(DroneState state)
        {
            State = state;
            _dispatcher.UpdateDroneStatus(new UpdateStatusDto
            {
                Id = Id,
                State = state.ToString()
            });
        }

        private void UpdateLocation()
        {
            _dispatcher.UpdateDroneStatus(new UpdateStatusDto
            {
                Id = Id,
                Location = Location
            });
        }

        public override string ToString()
        {
            return $"Drone:{{Id:{Id},Location:{Location},Destination:{Destination},State:{State}}}";
        }
        
        // Helper function for Haversine formula readability
        private static double ToRadians(double x) => Math.PI / 180 * x;

        // Helper function for Haversine formula readability
        private static double SinSquared(double x) => Math.Pow(Math.Sin(ToRadians(x) / 2), 2);

        // Calculate the difference between two points on a sphere
        private static double Haversine(
            double lat1, double lon1,
            double lat2, double lon2) =>
        EarthRadius * 2 * Math.Asin(
            Math.Sqrt(SinSquared(lat2 - lat1)) + SinSquared((lon2 - lon1) * Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))));
    }
}

