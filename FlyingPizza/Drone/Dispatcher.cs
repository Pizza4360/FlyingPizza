using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private record UrlRecord(string Url);
        public static async Task<string[]> GetDroneUrls()
        {
            //Todo: make sure you fix this the Kamron way...
            var url = "http://localhost:8080/Fleet?keys={url:1, _id:0}";
            var task = new RestDbSvc().Get<UrlRecord[]>(url);
            task.Wait();
            var objs = task.Result; 
            string[] arr = new string[task.Result.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = objs[i].Url;
            }
            return arr;
        }
        
        private void ServeForever()
        {
            
            /*Todod, loop forever, pausing every 2 seconds*/
            while (true)
            {
                /*Todod, check for orders and make it happen*/
                var activeOrders = GetActiveOrders().Result;
                if (activeOrders.Length > 0)
                {
                    /* Todo, when an order comes in, get a list of available drones
                     * and pick the first available DroneConnection to deliver the order. */
                    var task = new RestDbSvc().Get<DroneModel>(
                        "http://localhost:8080/Fleet?keys={url:1, _id:0}&filter={orderId:\"\"}");
                    task.Wait();
                    /*Todo and make sure order object is updated in database to "assigned".*/
                    /*Todo push the order to the drone via http protocol.*/
                }
                Thread.Sleep(2000);
            }
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
