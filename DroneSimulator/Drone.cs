using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Gateways;
using Domain.Entities;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;

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
        // The unique ID of this drone which is stored in the database
        private int Id { get; }

        // The point representing the pizza restaurant
        private GeoLocation Home { get; }

        // The current position of the drone
        private GeoLocation Location { get; set; }

        // The desired position of the drone
        public GeoLocation Destination { get; set; }

        // Current status of the drone
        private DroneState Status { get; set; }

        private readonly IDispatcherGateway _dispatcher;
        
        // Constructor
        public Drone(int id, GeoLocation Home, IDispatcherGateway dispatcher)
        {
            this.Id = id;
            Location = Home = Destination = Home;
            Status = DroneState.READY;
            _dispatcher = dispatcher;
        }

        // Calculate the next Point.X or Point.Y along a route
        decimal routeStep(decimal v, int i, int numberOfLocations, bool isLatitude) 
            => isLatitude && Home.Latitude == Destination.Latitude
                ? Home.Latitude
            : !isLatitude && Home.Longitude == Destination.Longitude
                ? Home.Longitude
            : (v * (i + 1)) / numberOfLocations;

        // Return an array of Point records simulating a drone's delivery route
        public GeoLocation[] GetRoute()
        {
            if (Home.Latitude == Destination.Latitude && Home.Longitude == Destination.Longitude)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            // Longitude distance to get to destination
            decimal xDistance = Home.Longitude - Destination.Longitude;

            // Latitude distance to get to destination
            decimal yDistance = Home.Latitude - Destination.Latitude;

            // # of locations should be the absolute value of the hypotenuse, rounded up to the
            // nearest integer
            int numberOfLocations = Math.Abs((int)Math.Ceiling(Math.Sqrt(
                (double)xDistance * (double)xDistance + (double)yDistance * (double)yDistance)));

            // LINQ yields all the points (except possibly the last one) along the route, one unit apart.
            List<GeoLocation> route = Enumerable.Range(0, numberOfLocations - 1)
                .Select(i => new GeoLocation
                {
                    Longitude = routeStep(xDistance, i, numberOfLocations, true),
                    Latitude = routeStep(yDistance, i, numberOfLocations, false)
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
            GeoLocation[] route = GetRoute();
            Console.WriteLine(this);

            for (int i = 0; i < route.Length; i++)
            {
                Location = route[i];
                UpdateLocation(Location);
                Console.WriteLine(this);
                Thread.Sleep(2000);
            }

            UpdateStatus(DroneState.RETURNING);
            for (int i = route.Length - 1; i >0; i--)
            {
                Location = route[i];
                UpdateLocation(Location);
                Console.WriteLine(this);
                Thread.Sleep(2000);
            }
            UpdateStatus(DroneState.READY);
        }

        private void UpdateStatus(DroneState state)
        {
            this.Status = state;
            _dispatcher.UpdateDroneStatus(new UpdateStatusDto
            {
                Status = state.ToString() //TODO: This probably won't work as expected
            });
            throw new NotImplementedException();
        }

        private void UpdateLocation(GeoLocation location)
        {
            _dispatcher.UpdateDroneStatus(new UpdateStatusDto
            {
                Location = location
            });
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"ID:{Id}\nlocation:{Location}\nDestination:{Destination}\nStatus:{Status}";
        }
    }
}
