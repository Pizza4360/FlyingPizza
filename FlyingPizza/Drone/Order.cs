using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FlyingPizza.Services;

namespace FlyingPizza.Drone
{
  public class Order
    {
      // The id to fetch the information from the database
        public string AllNewOrdersQueryUrl = "localhost:8080/Orders?keys={_id: 0, timeOrdered: 1,  timeDelivered: 1, deliveryAddress:1}&filter={badgeNumber: -1}";
        public JsonElement Document;
        private RestDbSvc Svc;
        private string OrderPage = "localhost:8080/Orders"; 
        public string Id { get; set; }
        public List<Object> Items {get; set;}
        public string Customer {get; set;}
        string DeliveryAddress {get; set;}
        public Point DeliveryLocation { get; } = new (0.0, 0.0);
        public int BadgeNumber {get; set;}
        
        // Todo: make these DateTime
        public string TimeOrdered {get; set;}
        public string TimeDelivered {get; set;}
        

        public override string ToString()
        {
          return 
          $"Items={Items}" +
          $"CustomerId={Customer}" +
          $"DeliveryAddress={DeliveryAddress}" +
          $"DeliveryLocation={DeliveryLocation}" +
          $"BadgeNumber={BadgeNumber}" +
          $"TimeOrdered={TimeOrdered}" +
          $"TimeDelivered={TimeDelivered}";
        }

        private int GetNextDrone()
        {
          throw new NotImplementedException();
        }


        public JsonElement.ArrayEnumerator GetDocument(string id)
        {
          var url = $"{OrderPage}/{id}?keys={{_id: 0, timeOrdered: 1, timeDelivered: 1, deliveryAddress:1}}";
          var entries = Svc.Get<JsonDocument>(url);
          entries.Wait();
          return entries.Result.RootElement.EnumerateArray();
        }
        // send update to the database
        public void UpdateRest(RestDbSvc svc = null)
        {
          if (svc == null)
          {
            svc = Svc;
          }
          svc.Put<Order>(OrderPage, this);
        }
        public async static Task<Order[]> GetOrders(int badgeNumber = -1)
        {
          string url = $"http://localhost:8080/Fleet?filter={{badgeNumber:{badgeNumber}}}";
            var task = await new RestDbSvc().Get<Order[]>(url);
            return task;
        }
    }
}
