using System;
using System.Linq;
using System.Net;
using System.Threading;
using FlyingPizza.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;


namespace FlyingPizza.Drone
{
    public class DroneRecord : ComponentBase
    {
        // The elapsed time between a drone getting to the next Point in a route
        private const int DroneUpdateInterval = 2000;
        // The drone statuses - Enums don't work with the REST service...
        public static string Ready = "Ready";
        public static string Delivering = "Delivering";
        public static string Returning = "Returning";
        public static string Dead = "Dead";
        public static string Charging = "Charging";
        
        // Connection to the database
        private RestDbSvc RestSvc { get; }
        // The prefix to every url query on the fleet of drones
        private const string FleetPage = "http://localhost:8080/Fleet/";
        // url to get to get the prototype for all new drones to be made from
        private const string DronePrototypeUrl = "http://localhost:8080/DefaultDrone/620058cfef84ea46a195b847";
        
        // The ip address to allow http communication to the dispatcher client 
        public string IpAddress { get; set; }
        // The unique ID of this drone which is stored in the database
        public int BadgeNumber { get; set; }

        // The current position of the drone
        public Point Location { get; set; } 
        
        public String OrderId { get; set; } 

        // The desired position of the drone
        public Point Delivery { get; set; }

        // Current status of the drone
        public string Status { get; set; }

        // The url to send commands to a drone
        public string Url { get; set; }

        public void Serve()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(IpAddress);
            listener.Start();
            while (true)
            {
                // From https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-6.0
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                // Todo: if there was a command from the dispatcher, handle it
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer,0,buffer.Length);
                // You must close the output stream.
                output.Close();
                Console.WriteLine(output);
            }
            listener.Stop();
        }
        
        // Make a new drone and add it to the database, then return it
        public static async Task<DroneRecord> AddDrone(string ipAddress)
        {
            // Instantiate a drone with the "DefaultDrone" prototype info
            var task = new RestDbSvc().Get<DroneRecord>(DronePrototypeUrl);
            task.Wait();
            DroneRecord dm = task.Result;
            
            // Set the new url for this drone's DB info and ip address for this drone's physical machine
            var responese = await new RestDbSvc().Post<DroneRecord>("http://localhost:8080/Fleet", dm);
            dm.Url = responese.Headers.Location.AbsoluteUri;
            dm.IpAddress = ipAddress;
            
            // Set the badge number to the next increasing integer  starting at 1
            var countTask = await new RestDbSvc().Get<DroneRecord[]>("http://localhost:8080/Fleet");
            dm.BadgeNumber = countTask.Length;
            
            // Put the updated drone information back
            new RestDbSvc().Put<DroneRecord>(dm.Url, dm);
            
            // Return the drone's url
            return await new RestDbSvc().Get<DroneRecord>(dm.Url);
        }

        // Get a drone by badge number
        public static DroneRecord GetDrone(int badgeNumber)
        {
            string url = $"http://localhost:8080/Fleet?filter={{badgeNumber:{badgeNumber}}}";
            var task = new RestDbSvc().Get<DroneRecord[]>(url);
            task.Wait();
            return task.Result[0];
        }

        // Return an array of Point records simulating a drone's delivery route
        public Point[] GetRoute()
        {
            if (Math.Abs(Location.Lat - Delivery.Lat) < Point.Tolerance 
                && Math.Abs(Location.Long - Delivery.Long) < Point.Tolerance)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            // Longitude distance to get to destination
            var xDistance = Location.Lat - Delivery.Lat;

            // Latitude distance to get to destination
            var yDistance = Location.Long - Delivery.Long;

            // # of steps should be the absolute value of the hypotenuse,
            // rounded up to the nearest integer
            var stepsCount = Math.Abs((int)Math.Ceiling(Math.Sqrt(
                xDistance * xDistance + yDistance * yDistance)));
            
            // The incremental change in latitude & longitude for each discrete
            // Point
            var xStep = Math.Abs(Delivery.Lat - Location.Lat) / stepsCount;
            var yStep = Math.Abs(Delivery.Long - Location.Long) / stepsCount;

            // The multiplier to ensure the direction of StepSize
            // increases for Destination X and Y > Home X and Y
            // decreases for Destination X and Y < Home X and Y
            var xDirection = Delivery.Lat > Location.Lat ? 1 : -1;
            var yDirection = Delivery.Long > Location.Long ? 1 : -1;
            
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
        public void DeliverOrder(string orderId)
        {
            string url = $"http://localhost:8080/Orders/{orderId}/?keys={{deliveryLocation:1, _id:0}}";
            var taskDelivery = RestSvc.Get<Point>(url);
            taskDelivery.Wait();
            Delivery = taskDelivery.Result;
            var route = GetRoute();
            OrderId = orderId;
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
            OrderId = "";
            UpdateStatus(Ready);
        }
        
        // Change the status of a drone and send update to database
        private void UpdateStatus(string state)
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
        public void UpdateRest(RestDbSvc svc = null)
        {
            if (svc == null)
            {
                svc = RestSvc;
            }
            svc.Put<DroneRecord>(FleetPage, this);
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
            DroneRecord oo = (DroneRecord) o;
            return oo.BadgeNumber == BadgeNumber &&
                   oo.Location.Equals(Location) &&
                   oo.Delivery.Equals(Delivery) &&
                   oo.Status.Equals(Status) &&
                   oo.Url.Equals(Url);
        }
    }
}
