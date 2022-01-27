using System;
using System.Linq;
using System.Threading;

namespace FlyingPizza.Drone
{
    public enum DroneState
    {
        Ready,
        Delivering,
        Returning,
        Dead,
        Charging
    }

    public sealed record Point(double X, double Y) 
    {
        public const double Tolerance = 0.0000001;
        public bool Equals(Point other) =>
            other != null
            && Math.Abs(X - other.X) < Tolerance
            && Math.Abs(Y - other.Y) < Tolerance;
        
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
 
    public class Drone
    {
        private const int DroneUpdateInterval = 2000;

        // The unique ID of this drone which is stored in the database
        private int Id { get; }

        // The point representing the pizza restaurant
        private Point Home { get; }

        // The current position of the drone
        private Point Location { get; set; } 

        // The desired position of the drone
        public Point Destination { get; set; }

        // Current status of the drone
        private DroneState Status { get; set; }
        
        // Constructor
        public Drone(int id, Point home)
        {
            Id = id;
            Location = Home = Destination = home;
            Status = DroneState.Ready;
        }

        // Return an array of Point records simulating a drone's delivery route
        public Point[] GetRoute()
        {
            if (Math.Abs(Home.X - Destination.X) < Point.Tolerance 
                && Math.Abs(Home.Y - Destination.Y) < Point.Tolerance)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            // Longitude distance to get to destination
            var xDistance = Home.X - Destination.X;

            // Latitude distance to get to destination
            var yDistance = Home.Y - Destination.Y;

            // # of steps should be the absolute value of the hypotenuse,
            // rounded up to the nearest integer
            var stepsCount = Math.Abs((int)Math.Ceiling(Math.Sqrt(
                xDistance * xDistance + yDistance * yDistance)));
            
            // The incremental change in latitude & longitude for each discrete
            // Point
            var xStep = Math.Abs(Destination.X - Home.X) / stepsCount;
            var yStep = Math.Abs(Destination.Y - Home.Y) / stepsCount;

            // The multiplier to ensure the direction of StepSize
            // increases for Destination X and Y > Home X and Y
            // decreases for Destination X and Y < Home X and Y
            var xDirection = Destination.X > Home.X ? 1 : -1;
            var yDirection = Destination.Y > Home.Y ? 1 : -1;
            
            Point[] route = new Point[stepsCount];

            for (var i = 0; i < stepsCount - 1; i++)
            {
                route[i] = new Point((i + 1) * xStep * xDirection,
                    (i + 1) * yStep * yDirection);
            }

            route[stepsCount - 1] = Destination;
            return route;
        }

        // Dispatch a drone to deliver a pizza.
        public void DeliverOrder(Point customerLocation)
        {
            Destination = customerLocation;
            var route = GetRoute();
            UpdateStatus(DroneState.Delivering);
            
            // Travel to Destination
            foreach (var point in route)
            {
                UpdateLocation(point, DroneUpdateInterval);
            }
            UpdateStatus(DroneState.Returning);
            
            // Travel back to Home
            foreach (var point in route.Reverse())
            {
                UpdateLocation(point, DroneUpdateInterval);
            }
            UpdateStatus(DroneState.Ready);
        }

        private void UpdateStatus(DroneState state)
        {
            Status = state;
            // Todo: Post a status update to the database.
        }

        private void UpdateLocation(Point location, int sleepTime = 0)
        {
            Location = location;
            Console.WriteLine(this);
            Thread.Sleep(sleepTime);
            // Todo: Post a location update to the database. 
        }

        public override string ToString()
        {
            return $"ID:{Id}\n" +
                   $"location:{Location}\n" +
                   $"Destination:{Destination}\n" +
                   $"Status:{Status}";
        }
    }
}
