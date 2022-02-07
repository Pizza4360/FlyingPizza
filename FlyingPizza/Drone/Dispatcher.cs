using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FlyingPizza.Drone;
using FlyingPizza.Services;

namespace DroneDispatcher
{
    public sealed record DroneConnection(HttpClient HttpConnection, int BadgeNumber);
    public class Dispatcher
    {
        private Queue<KeyValuePair<int , string>> ReadyDrones;
        private Dictionary<int, string> WorkingDrones;
        private static RestDbSvc Svc = new RestDbSvc();
        private static readonly HttpClient Client = new HttpClient();
        
        public Dispatcher(Dictionary<int, string> droneConnections)
        {
            WorkingDrones = new Dictionary<int, string>();
            ReadyDrones = new Queue<KeyValuePair<int, string>>();
            foreach (KeyValuePair<int, string> server in droneConnections)
            { 
                ReadyDrones.Enqueue(server);
            }
        }

        private record UrlRecord(string Url);
        public static async Task<string[]> GetDroneUrls()
        {
            //Todo: make sure you fix this the Kamron way...
            var url = "http://localhost:8080/Fleet?keys={url:1, _id:0}";
            var task = Svc.Get<UrlRecord[]>(url);
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
                foreach (Order order in GetActiveOrders().Result)
                {
                    var droneConnection = ReadyDrones.Dequeue();
                    WorkingDrones.Add(droneConnection.Key, droneConnection.Value);
                    var task = Svc.Get<DroneModel>(
                        "http://localhost:8080/Fleet?keys={url:1, _id:0}&filter={orderId:\"\"}");
                    task.Wait();
                    /*Todod push the order to the drone via http protocol.*/
                    string response = SendDeliveryCommand(droneConnection.Value, order,task.Result).Result;
                }
                Thread.Sleep(2000);
            }
        }

        private async Task<string> SendDeliveryCommand(string serverUrl, Order order, 
        DroneModel drone)
        {
            
            var parameters = new Dictionary<string, string>();
            parameters["text"] = order.Id;
            parameters["command"] = DroneModel.Delivering;
            var response = await Client.PostAsync(serverUrl, new FormUrlEncodedContent(parameters));
            var contents = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            
            // Update the order with the drone's badgeNumber
            order.BadgeNumber = drone.BadgeNumber;
            order.UpdateRest();
            
            // Update the drone with status "Delivering" and new order's orderId
            drone.Status = DroneModel.Delivering;
            drone.OrderId = order.Id;
            drone.UpdateRest(Svc);
            return contents;
        }

        public async Task<Order[]> GetActiveOrders()
        {
            var url = "http://localhost:8080/Orders"; 
            RestDbSvc  r = Svc;
            var entries = r.Get<Order[]>(url);
            entries.Wait();
            Console.WriteLine("r status:" + entries);
            return entries.Result;
        }
    }
}
