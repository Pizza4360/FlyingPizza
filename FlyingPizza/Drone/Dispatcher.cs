using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FlyingPizza.Drone;
using FlyingPizza.Services;

namespace DroneDispatcher
{
    public class DroneConnection
    {
        private string Url;
        public DroneConnection(string url)
        {
            Url = url;
        }
        
        public string GetStatus(string status)
        {
            // Todo retrieve status form db with query
            throw new NotImplementedException();
        }
        
        public void AssignOrder(Order order)
        {
            // Todo send message to drone to make it call DeliverOrder()
            throw new NotImplementedException();
        }
        
        public Order GetOrder()
        {
            // Todo get order info 
            throw new NotImplementedException();
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
