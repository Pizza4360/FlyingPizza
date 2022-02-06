using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading;
using FlyingPizza.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Xunit.Sdk;


namespace FlyingPizza.Drone
{
    // An object representing the subset of attributes that will describe a drone in the database's Fleet page
    public class DroneModel : ComponentBase
    {
        // The elapsed time between a drone getting to the next Point in a route
        private const int DroneUpdateInterval = 2000;

        // The prefix to every url query on the fleet of drones
        private const string FleetPage = "http://localhost:8080/Fleet/";
        
        // The unique ID of this drone which is stored in the database
        public int BadgeNumber { get; }
        
        // The current position of the drone
        public Point Location { get; set; } 

        // The desired position of the drone
        public Point Delivery { get; set; }

        // Current status of the drone
        public DroneStatus Status { get; set; }

        // The Rest Service used for each drone
        private RestDbSvc RestSvc { get;}
        
        // The url to send commands to a drone
        //TODO: Removed since updating it upon construction of itself is becoming tedious
        //private string Url { get; set; }
        
        
        // Constructor
        public DroneModel(int badgeNumber, Point location)
        {
            BadgeNumber = badgeNumber;
            Location = location;
            Delivery = location;
            Status =  new DroneStatus(DroneState.Ready);
            RestSvc = new RestDbSvc();
        }

        // atomic write of drone to database (hopefully)
        public async Task writeDb()
        {
            var badgesTask = await RestSvc.Get<int[]>("http://localhost:8080/Fleet?filter={badgeNumber: " + BadgeNumber + 
                                                "}&keys={badgeNumber:1}");
            if (badgesTask.Length >= 1)
            {
                RestSvc.Put(FleetPage, this);
            }
            else
            {
                RestSvc.Post(FleetPage, this);
            }
        }
        // Atomic read (hopefully) of drone record
        public async Task<DroneModel[]> readDbArray()
        {
            var readTask = await RestSvc.Get<DroneModel[]>(FleetPage + "?filter={badgeNumber: " + BadgeNumber + "}");
            return readTask;
        }
        // Return an array of Point records simulating a drone's delivery route
        public Point[] GetRoute()
        {

            var dest = Delivery;
            if (Math.Abs(Location.X - dest.X) < Point.Tolerance 
                && Math.Abs(Location.Y - dest.Y) < Point.Tolerance)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            // Longitude distance to get to destination
            var xDistance = Location.X - dest.X;

            // Latitude distance to get to destination
            var yDistance = Location.Y - dest.Y;

            // # of steps should be the absolute value of the hypotenuse,
            // rounded up to the nearest integer
            var stepsCount = Math.Abs((int)Math.Ceiling(Math.Sqrt(
                xDistance * xDistance + yDistance * yDistance)));
            
            // The incremental change in latitude & longitude for each discrete
            // Point
            var xStep = Math.Abs(dest.X - Location.X) / stepsCount;
            var yStep = Math.Abs(dest.Y - Location.Y) / stepsCount;

            // The multiplier to ensure the direction of StepSize
            // increases for Destination X and Y > Home X and Y
            // decreases for Destination X and Y < Home X and Y
            var xDirection = dest.X > Location.X ? 1 : -1;
            var yDirection = dest.Y > Location.Y ? 1 : -1;
            
            Point[] route = new Point[stepsCount];

            for (var i = 0; i < stepsCount - 1; i++)
            {
                route[i] = new Point((i + 1) * xStep * xDirection,
                    (i + 1) * yStep * yDirection);
            }

            route[stepsCount - 1] = dest;
            return route;
        }

        // Dispatch a drone to deliver a pizza.
        public void DeliverOrder(Point customerOrder)
        {
            Delivery = customerOrder;
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
        
        // Change the status of a drone and send update to database
        private void UpdateStatus(DroneState state)
        {
            Status = new DroneStatus(state);
            // Post a status update to the database.
            //writeDb(true).Wait();
        }

        // Change the location of a drone and send update to database
        private void UpdateLocation(Point location, int sleepTime = 0)
        {
            Location = location;
            Console.WriteLine(this);
            Thread.Sleep(sleepTime);
            // Post a location update to the database.
            writeDb().Wait();
        }
        
        // send update to the database

        // String for debugging purposes
        public override string ToString()
        {
            return $"badgeNumber:{BadgeNumber}\n" +
                   $"location:{Location}\n" +
                   $"Destination:{Delivery}\n" +
                   $"Status:{Status}";
        }
        
        // Equality for testing purposes
        public override bool Equals(object obj)
        {
            DroneModel test = obj as DroneModel;
            if (test == null)
            {
                return false;
            }
            else
            {
                if (test.Delivery == this.Delivery && test.Location == this.Location &&
                    test.BadgeNumber == this.BadgeNumber)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    
}
