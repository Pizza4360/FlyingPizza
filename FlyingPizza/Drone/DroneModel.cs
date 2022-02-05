using System;
using System.Linq;
using System.Threading;
using FlyingPizza.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Xunit.Sdk;


namespace FlyingPizza.Drone
{
    public sealed record DroneRecord(int FleetNumber, string Id, Point Location, DroneState State)
    {
        public static DroneRecord From(DroneModel model)
        {
            return new(model.FleetNumber, model.Id, model.Location, model.Status);
        }
    }
    public class DroneModel : ComponentBase
    {
        private const int DroneUpdateInterval = 2000;

        // The unique ID of this drone which is stored in the database
        //TODO: DewsDavid31 - I changed this to public for our sanity in testing/database
        public int FleetNumber { get; }

        // The point representing the pizza restaurant
        private Point Home { get; }

        // The current position of the drone
        //TODO: DewsDavid31 - I changed this to public for our sanity in testing/database
        public Point Location { get; set; } 

        // The desired position of the drone
        public Point Destination { get; set; }

        // Current status of the drone
        //TODO: DewsDavid31 - I changed this to public for our sanity in testing/database
        public DroneState Status { get; set; }

        private RestDbSvc RestSvc { get;}
        
        private string Url { get; set; }
        public string Id { get; set; }
        
        private string FleetPage = "http://localhost:8080/Fleet/";

        // Constructor
        public DroneModel(int fleetNumber, Point home, string Id = "")
        {
            FleetNumber = fleetNumber;
            Location = Home = Destination = home;
            Status = DroneState.Ready;

            // This may not work unless you ssh into mongo server
            RestSvc = new RestDbSvc();
            if (Id.Equals("")) RegisterDrone();
        }

        private async Task RegisterDrone()
        {
            // Todo: register drone if not in the DB this creates duplicates
            FleetPage = "http://localhost:8080/Fleet/";
            var response = await RestSvc.Post<DroneRecord>(FleetPage, DroneRecord.From(this));
            if (response.Headers.Location is not null) Url = response.Headers.Location.AbsoluteUri;
            else throw new NullException("Something went wrong here");
            var task = RestSvc.Put(Url, this);
            Console.WriteLine(task + "");
            
            // Todo make sure you get the Id back and assign it here
            Id = task + "";
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
            // Post a status update to the database.
            updateRest();
        }

        private void UpdateLocation(Point location, int sleepTime = 0)
        {
            Location = location;
            Console.WriteLine(this);
            Thread.Sleep(sleepTime);
            // Post a location update to the database.
            updateRest();
        }
        //TODOd: needs Rest to be SSH'd into to use, also silently fails to upsert a record.
        // Puts a Drone object into MongoDB, used to put separate fields instead, but it didn't work
        // CORRECTION: neither does this, sadly...
        private void updateRest()
        {
            RestSvc.Post<DroneRecord>(FleetPage, DroneRecord.From(this));
        }
        public override string ToString()
        {
            return $"ID:{FleetNumber}\n" +
                   $"location:{Location}\n" +
                   $"Destination:{Destination}\n" +
                   $"Status:{Status}";
        }
    }
    
}
