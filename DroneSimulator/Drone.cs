using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Gateways;
using Domain.Entities;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Microsoft.AspNetCore.Routing;
using static System.Decimal;

namespace DroneSimulator
{
    public enum DroneState
    {
        READY,
        DELIVERING,
        RETURNING,
        DEAD,
        CHARGING
    }

    public class Drone
    {
        private const double Drone_Speed = 0.0089408;

        private const double Tick_Count = 2.00;
        // The unique ID of this drone which is stored in the database
        private string Id { get; }

        // The point representing the pizza restaurant
        private GeoLocation Home { get; }

        // The current position of the drone
        private GeoLocation Location { get; set; }

        // The desired position of the drone
        public GeoLocation Destination { get; set; }

        // Current status of the drone
        private DroneState State { get; set; }

        private readonly IDispatcherGateway _dispatcher;

        // Constructor
        public Drone(string id, GeoLocation Home, IDispatcherGateway dispatcher)
        {
            Id = id;
            Location = Home;
            Destination = Home;
            State = DroneState.READY;
            _dispatcher = dispatcher;
            this.Home = Home;
        }

        // Calculate the next Point.X or Point.Y along a route
        // decimal routeStep(decimal v, int i, int numberOfLocations, bool isLatitude)
        //     => isLatitude && Home.Latitude == Destination.Latitude
        //         ? Home.Latitude
        //         : !isLatitude && Home.Longitude == Destination.Longitude
        //             ? Home.Longitude
        //             : (v * (i + 1)) / numberOfLocations;

        // Return an array of Point records simulating a drone's delivery route
        public GeoLocation[] GetRoute()
        {
            if (Home.Latitude == Destination.Latitude && Home.Longitude == Destination.Longitude)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }


            // # of locations should be the absolute value of the hypotenuse, rounded up to the
            // nearest integer

            var distance = haversine(ToDouble(Home.Latitude), ToDouble(Home.Longitude),
                ToDouble(Destination.Latitude), ToDouble(Destination.Longitude));
            var step_size = Tick_Count * Drone_Speed;
            
            var numberOfLocations = (int)Math.Floor(distance / step_size);

            Console.WriteLine($"need to travel {distance} km, step_size={step_size}, num locations={numberOfLocations}");
            // Longitude distance to get to destination
            var xStep = (Home.Longitude - Destination.Longitude) / numberOfLocations;
            
            // Latitude distance to get to destination
            var yStep = (Home.Latitude - Destination.Latitude) / numberOfLocations;
            // LINQ yields all the points (except possibly the last one) along the route, one unit apart.
            List<GeoLocation> route = Enumerable.Range(0, numberOfLocations - 1)
                .Select(i => new GeoLocation
                {
                    Latitude = i * xStep,
                    Longitude = i * yStep
                }).ToList();

            // Add the Destination Point if needed.
            if (!route.TakeLast(1).Equals(Destination))
            {
                route.Add(Destination);
            }

            return route.ToArray();
        }

        // Dispatch a drone to deliver a pizza.
        public void deliverOrder(GeoLocation customerLocation)
        {
            Destination = customerLocation;
            UpdateStatus(DroneState.DELIVERING);
            var route = GetRoute();
            var s = string.Join(",", route.Select(x => $"{{{x.Latitude},{x.Longitude}}}").ToArray());
            Console.WriteLine($"{this},route={s}");

            for (int i = 0; i < route.Length; i++)
            {
                Location = route[i];
                UpdateLocation(Location);
                Console.WriteLine(this);
                Thread.Sleep(2000);
            }

            Console.WriteLine("Order complete!");
            UpdateStatus(DroneState.RETURNING);
            for (int i = route.Length - 1; i > 0; i--)
            {
                Location = route[i];
                UpdateLocation(Location);
                Console.WriteLine(this);
                Thread.Sleep(2000);
            }

            UpdateStatus(DroneState.READY);
            Console.WriteLine("Back home!");
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

        private void UpdateLocation(GeoLocation location)
        {
            _dispatcher.UpdateDroneStatus(new UpdateStatusDto
            {
                Id = Id,
                Location = location
            });
        }

        public override string ToString()
        {
            var loc = $"{{{Location.Latitude},{Location.Longitude}}}";
            var dest = $"{{{Destination.Latitude},{Destination.Longitude}}}";
            return $"ID:{Id}\nlocation:{loc}\nDestination:{dest}\nStatus:{State}";
        }

        static double reversHaversine(double lat1, double lat2, double lon1, double lon2)
        {
            return 2 * 6371000 * Math.Asin(Math.Sqrt((lat2 - lat2) / 2) + Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Pow(Math.Sin((lon2 - lon1) / 2), 2));
        }
        static double haversine(double lat1, double lon1,
            double lat2, double lon2)
        {
            // distance between latitudes and longitudes
            double dLat = (Math.PI / 180) * (lat2 - lat1);
            double dLon = (Math.PI / 180) * (lon2 - lon1);

            // convert to radians
            lat1 = (Math.PI / 180) * (lat1);
            lat2 = (Math.PI / 180) * (lat2);

            // apply formulae
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Pow(Math.Sin(dLon / 2), 2) *
                       Math.Cos(lat1) * Math.Cos(lat2);
            double rad = 6371;
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return rad * c;
        }
    }
}

