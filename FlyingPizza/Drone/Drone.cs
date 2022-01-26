using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace FlyingDrone
{
    public enum DroneState
    {
        READY,
        DELIVERING,
        RETURNING,
        DEAD,
        CHARGING
    }

    public class Point
    {
        public Point(double X, double Y)
    {
        this.X = X;
        this.Y = Y;
    }

        public double X { get; set; }
        public double Y { get; set; }

        public override bool Equals(object? obj)
        {
            return obj.GetType() == typeof(Point)
                   && Math.Abs(X - ((Point) obj).X) < Drone.TOLERANCE
                   && Math.Abs(Y - ((Point) obj).Y) < Drone.TOLERANCE;
        }
    }
 
    public class Drone
    {
        public const double TOLERANCE = 0.000001;

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
            Status = DroneState.READY;
        }

        // Calculate the next Point.X or Point.Y along a route
        double routeStep(double v, int i, int numberOfLocations, bool isLatitude) 
            => isLatitude && Math.Abs(Home.X - Destination.X) < TOLERANCE
                ? Home.X
            : !isLatitude && Math.Abs(Home.Y - Destination.Y) < TOLERANCE
                ? Home.Y
            : (v * (i + 1)) / numberOfLocations;

        // Return an array of Point records simulating a drone's delivery route
        public Point[] GetRoute()
        {
            if (Math.Abs(Home.X - Destination.X) < TOLERANCE && Math.Abs(Home.Y - Destination.Y) < TOLERANCE)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            // Longitude distance to get to destination
            double xDistance = Home.X - Destination.X;

            // Latitude distance to get to destination
            double yDistance = Home.Y - Destination.Y;

            // # of locations should be the absolute value of the hypotenuse, rounded up to the
            // nearest integer
            int numberOfLocations = Math.Abs((int)Math.Ceiling(Math.Sqrt(
                xDistance * xDistance + yDistance * yDistance)));

            double xStepSize = Math.Abs(Destination.X - Home.X) / numberOfLocations;
            double yStepSize = Math.Abs(Destination.Y - Home.Y) / numberOfLocations;

            int xDirection = Destination.X > Home.X ? 1 : -1;
            int yDirection = Destination.Y > Home.Y ? 1 : -1;
            
            Point[] route = new Point[numberOfLocations];

            for (int i = 0; i < numberOfLocations - 1; i++)
            {
                route[i] = new((i + 1) * xStepSize * xDirection,(i + 1) * yStepSize * yDirection);
            }

            route[numberOfLocations - 1] = Destination;

            // // LINQ yields all the points (except possibly the last one) along the route, one unit apart.
            // List<Point> route = Enumerable.Range(0, numberOfLocations - 1)
            //     .Select(i => new Point(
            //         routeStep(xDistance, i, numberOfLocations, true),
            //         routeStep(yDistance, i, numberOfLocations, false)
            //     )).ToList();

            // Add the Destination Point if needed.
            // if (!route.TakeLast(1).Equals(Destination))
            // {
            //     route.Add(Destination);
            // }
            // return route.ToArray();
            return route;
        }

        // Dispatch a drone to deliver a pizza.
        public void deliverOrder(Point customerLocation)
        {
            Destination = customerLocation;
            UpdateStatus(DroneState.DELIVERING);
            Point[] route = GetRoute();
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

        // Todo: Post a status update to the database.
        private void UpdateStatus(DroneState state)
        {
            this.Status = state;
            throw new NotImplementedException();
        }

        // Todo: Post a location update to the database. 
        private void UpdateLocation(Point location)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"ID:{Id}\nlocation:{Location}\nDestination:{Destination}\nStatus:{Status}";
        }
    }
}
