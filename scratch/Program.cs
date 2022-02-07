using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FlyingPizza.Drone;
using FlyingPizza.Services;

namespace scratch
{
    class Program
    {
        public async static Task Main(string[] args)
        {
            Order[] orders = Order.GetOrders().Result;
            Console.WriteLine(orders);
        }

        private static string[] GetDroneUrls()
        {
            // Todo make this return a Dictionary of {_id:url}
            RestDbSvc r = new RestDbSvc();
            var url = "http://localhost:8080/Fleet?keys={'_id': 1, 'url':1}";
            var entries = r.Get<JsonDocument>(url);
            entries.Wait();
            return entries.Result.RootElement.EnumerateArray()
                .Select(
                    _ => _.GetProperty("url").ToString()
                )
                .ToArray();
        }
        private static bool droneExists(int badgeNumber)
        {
            // Todo make this return a Dictionary of {_id:url}
            RestDbSvc r = new RestDbSvc();
            var url = $"http://localhost:8080/Fleet?keys={{_id: 0, badgeNumber: {badgeNumber}}}";
            var entries = r.Get<JsonDocument>(url);
            entries.Wait();
            return entries.Result.RootElement.EnumerateArray()
                .Select(
                    _ => _.GetProperty("url").ToString()
                )
                .ToArray().Length == 1;
        }
    }

    public sealed record DroneRecords(List<DroneRecord> records){}
    public sealed record DroneRecord(string Url)
    {}

}
