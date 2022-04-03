﻿using System;
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
        private const decimal StepSize = DroneUpdateInterval / 1000.0m * (decimal)DroneSpeed;
        
        // The unique ID of this drone which is stored in the database
        private string Id { get; }

        // The point representing the pizza restaurant
        public GeoLocation Home { get; set; }

        // The current position of the drone
        public GeoLocation Location { get; set; }

        // The desired position of the drone
        public GeoLocation Destination { get; set; }

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
        
        // Return an array of Geolocations representing a drone's delivery route
        public GeoLocation[] GetRoute()
        {
            if (Home.Equals(Destination))
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            var distance = Haversine(ToDouble(Home.Latitude), ToDouble(Home.Longitude),
                ToDouble(Destination.Latitude), ToDouble(Destination.Longitude));

            var numberOfLocations = (int)Math.Floor((decimal)distance / StepSize);

            Console.WriteLine($"need to travel {distance} km, step_size={StepSize}, num locations={numberOfLocations}");
            // Latitude distance to get to destination
            var xStep = (Destination.Longitude - Home.Longitude) / numberOfLocations;
            
            // Longitude distance to get to destination
            var yStep = (Destination.Latitude - Home.Latitude) / numberOfLocations;
            
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

        // Tell Drone to deliver an order
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
        private void UpdateDroneState(DroneState state)
        {
            State = state;
            _dispatcher.PutDroneState(
                new UpdateStatusDto
                {
                    Id = Id,
                    State = $"{state}"
                });
        }

        // Send an Location update to DispatcherGateway
        private void UpdateLocation(GeoLocation location)
        {
            Location = location;
            _dispatcher.PutDroneState(
                new UpdateStatusDto
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

