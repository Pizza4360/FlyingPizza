using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlyingPizza.Drone;
using FlyingPizza.Services;

namespace DroneDispatcher
{
    public class DroneConnection
    {
        private string Url;
        private int BadgeNumber;
        public DroneConnection(string url, int badgeNumber)
        {
            Url = url;
            BadgeNumber = badgeNumber;
        }
        
        public async Task<string> GetStatus()
        {
            return await new RestDbSvc().Get<string>($"{Url}?keys={{status:1, _id:0}}");
        }
        
        public void AssignOrder(Order order)
        {
            var drone = DroneModel.GetDrone(BadgeNumber);
            drone.Delivery = order.DeliveryLocation;
            new RestDbSvc().Put(Url, drone);
        }
    }
    
    public class Dispatcher
    {
        private List<DroneConnection> Drones;

        public Dispatcher(){}

        private static string[] GetDroneUrls()
        {
            // // Todo make this return a Dictionary of {_id:url}
            // RestDbSvc r = new RestDbSvc();
            // var url = "http://localhost:8080/Fleet?keys={'_id': 1, 'url':1}";
            // var entries = r.Get<JsonDocument>(url);
            // entries.Wait();
            // entries.Result.RootElement.
            // return entries.Result.RootElement.EnumerateArray()
            //     .Select(
            //         _ => _.GetProperty("url").ToString()
            //     )
            //     .ToArray();
            return new[] {""};
        }
        
        private void ServeForever()
        {
            /*Todo, check for orders and make it happen*/
            
            /*Todo, loop forever, pausing every 2 seconds*/
            
            /* Todo, when an order comes in, get a list of available drones
             * and pick the first available DroneConnection to deliver the order. */
            
            /*Todo and make sure order object is updated in database to "assigned"*/
        }

        public async Task<Order[]> GetActiveOrders()
        {
            var url = "http://localhost:8080/Orders"; 
            RestDbSvc  r = new RestDbSvc();
            var entries = r.Get<Order[]>(url);
            entries.Wait();
            Console.WriteLine("r status:" + entries);
            return entries.Result;
        }
    }
}
