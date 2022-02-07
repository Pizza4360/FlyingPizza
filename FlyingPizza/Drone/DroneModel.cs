~using System;
using System.Linq;
using System.Threading;
using FlyingPizza.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;


namespace FlyingPizza.Drone
{
    public class DroneModel : ComponentBase
    {
        // The elapsed time between a drone getting to the next Point in a route
        private const int DroneUpdateInterval = 2000;
        
        public static string Ready = "Ready";
        public static string Delivering = "Delivering";
        public static string Returning = "Returning";
        public static string Dead = "Dead";
        public static string Charging = "Charging";
        
        // The prefix to every url query on the fleet of drones
        private const string FleetPage = "http://localhost:8080/Fleet/";
        
        // The unique ID of this drone which is stored in the database
        public int BadgeNumber { get; set; }

        // The current position of the drone
        public Point Location { get; set; } 
        
        
        public String OrderId { get; set; } 

        // The desired position of the drone
        public Point Delivery { get; set; }

        // Current status of the drone
        public string Status { get; set; }

        // The singleton REST service used for all drones
        private RestDbSvc RestSvc { get;}
        
        // The url to send commands to a drone
        public string Url { get; set; }
        
        // Make a new drone and add it to the database, then return it
        public static async Task<DroneModel> AddDrone()
        {
            string url = "http://localhost:8080/DefaultDrone/620058cfef84ea46a195b847";
            var task = new RestDbSvc().Get<DroneModel>(url);
            task.Wait();
            DroneModel dm = task.Result;
            Console.WriteLine(dm);
            
            var responese = await new RestDbSvc().Post<DroneModel>("http://localhost:8080/Fleet", dm);
            dm.Url = responese.Headers.Location.AbsoluteUri;
            var countTask = await new RestDbSvc().Get<DroneModel[]>("http://localhost:8080/Fleet");
            dm.BadgeNumber = countTask.Length;
            new RestDbSvc().Put<DroneModel>(dm.Url, dm);
            return await new RestDbSvc().Get<DroneModel>(dm.Url);
        }

        // Get a drone by badge number
        public static DroneModel GetDrone(int badgeNumber)
        {
            string url = $"http://localhost:8080/Fleet?filter={{badgeNumber:{badgeNumber}}}";
            var task = new RestDbSvc().Get<DroneModel[]>(url);
            task.Wait();
            return task.Result[0];
        }

        // Return an array of Point records simulating a drone's delivery route
        public Point[] GetRoute()
        {
            if (Math.Abs(Location.X - Delivery.X) < Point.Tolerance 
                && Math.Abs(Location.Y - Delivery.Y) < Point.Tolerance)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            // Longitude distance to get to destination
            var xDistance = Location.X - Delivery.X;

            // Latitude distance to get to destination
            var yDistance = Location.Y - Delivery.Y;

            // # of steps should be the absolute value of the hypotenuse,
            // rounded up to the nearest integer
            var stepsCount = Math.Abs((int)Math.Ceiling(Math.Sqrt(
                xDistance * xDistance + yDistance * yDistance)));
            
            // The incremental change in latitude & longitude for each discrete
            // Point
            var xStep = Math.Abs(Delivery.X - Location.X) / stepsCount;
            var yStep = Math.Abs(Delivery.Y - Location.Y) / stepsCount;

            // The multiplier to ensure the direction of StepSize
            // increases for Destination X and Y > Home X and Y
            // decreases for Destination X and Y < Home X and Y
            var xDirection = Delivery.X > Location.X ? 1 : -1;
            var yDirection = Delivery.Y > Location.Y ? 1 : -1;
            
            Point[] route = new Point[stepsCount];

            for (var i = 0; i < stepsCount - 1; i++)
            {
                route[i] = new Point((i + 1) * xStep * xDirection,
                    (i + 1) * yStep * yDirection);
            }

            route[stepsCount - 1] = Delivery;
            return route;
        }

        // Dispatch a drone to deliver a pizza.
        public void DeliverOrder(String orderId)
        {
            string url = $"http://localhost:8080/Orders/{orderId}/?keys={{deliveryLocation:1, _id:0}}";
            var taskDelivery = RestSvc.Get<Point>(url);
            taskDelivery.Wait();
            Delivery = taskDelivery.Result;
            var route = GetRoute();
            UpdateStatus(Delivering);
            
            // Travel to Destination
            foreach (var point in route)
            {
                UpdateLocation(point, DroneUpdateInterval);
            }
            UpdateStatus(Returning);
            
            // Travel back to Home
            foreach (var point in route.Reverse())
            {
                UpdateLocation(point, DroneUpdateInterval);
            }
            UpdateStatus(Ready);
        }
        
        // Change the status of a drone and send update to database
        private void UpdateStatus(String state)
        {
            // if(state.Equals())
            Status = state;
            // Post a status update to the database.
            UpdateRest();
        }

        // Change the location of a drone and send update to database
        private void UpdateLocation(Point location, int sleepTime = 0)
        {
            Location = location;
            Console.WriteLine(this);
            Thread.Sleep(sleepTime);
            // Post a location update to the database.
            UpdateRest();
        }
        
        // send update to the database
        private void UpdateRest()
        {
            RestSvc.Put<DroneModel>(FleetPage, this);
        }
        
        // String for debugging GetDronepurposes
        public override string ToString()
        {
            return $"ID:{BadgeNumber}\n" +
                   $"location:{Location}\n" +
                   $"Destination:{Delivery}\n" +
                   $"Status:{Status}";
        }

        public override bool Equals(object o)
        {
            if (o == null || o.GetType() != GetType()) return false;
            DroneModel oo = (DroneModel) o;
            return oo.BadgeNumber == BadgeNumber &&
                   oo.Location.Equals(Location) &&
                   oo.Delivery.Equals(Delivery) &&
                   oo.Status.Equals(Status) &&
                   oo.Url.Equals(Url);
        }
    }
    
    
}
