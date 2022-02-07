using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FlyingPizza.Services;

namespace FlyingPizza.Drone
{
  public class Order
    {
      // The id to fetch the information from the database
        public string AllNewOrdersQueryUrl = "localhost:8080/Orders?keys={_id: 0, timeOrdered: 1,  timeDelivered: 1, deliveryAddress:1}&filter={badgeNumber: -1}";
        public JsonElement Document;
        private RestDbSvc Svc;
        
        public Object OrderId {get; set;}
        public List<Object> Items {get; set;}
        public string CustomerId {get; set;}
        string DeliveryAddress {get; set;}
        public Point DeliveryLocation { get; } = new (0.0, 0.0);
        public int BadgeNumber {get; set;}
        
        // Todo: make these DateTime
        public string TimeOrdered {get; set;}
        public string TimeDelivered {get; set;}
        

        public override string ToString()
        {
          return 
          $"OrderId={OrderId}" +
          $"Items={Items}" +
          $"CustomerId={CustomerId}" +
          $"DeliveryAddress={DeliveryAddress}" +
          $"DeliveryLocation={DeliveryLocation}" +
          $"BadgeNumber={BadgeNumber}" +
          $"TimeOrdered={TimeOrdered}" +
          $"TimeDelivered={TimeDelivered}";
        }
        
        public string testString = @"
{
  ""_id"": { ""$oid"": ""61fd502d88f4e36ecd043989"" },
  ""items"": [
    {
      ""type"": ""pizza"",
      ""toppings"": [""pepperoni"", ""mushrooms"", ""onion""],
      ""size"": ""medium"",
      ""price"": 13.42
    },
    {
      ""type"": ""pizza"",
      ""toppings"": [""bell peppers"", ""mushrooms"", ""onion""],
      ""size"": ""medium"",
      ""price"": 13.42
    }
  ],
  ""customer"": 30498,
  ""time ordered"": ""02:34:01:25:2022"",
  ""time delivered"": ""02:58:01:25:2022"",
  ""deliveryAddress"": ""3920 Tennyson St. Denver, CO 80212"",
  ""deliveryLocation"": ""39.77147908776442, -105.04382896161589"",
  ""badgeNumber"": ""-1""
}";
        
        
        // public Order(string id)
        // {
        //   if (id.Equals(""))
        //   {
        //     throw new Exception("need to make a document for manual entry");
        //   }
        //   Svc = new RestDbSvc();
        //   var arr = GetDocument(id).ToArray();
        //   if (arr.Length != 1)
        //   {
        //     throw new Exception($"There should be exactly one order with id '{id}'");
        //   }
        //   foreach (JsonElement element in arr)
        //   {
        //     TimeOrdered = DateTime.Parse(element.GetProperty("timeOrdered") + "");
        //     DeliveryAddress = element.GetProperty("deliveryAddress") + "";
        //   } 
        //   BadgeNumber = GetNextDrone();
        //   DeliveryLocation = AddressLookup();
        // }

        private int GetNextDrone()
        {
          throw new NotImplementedException();
        }


        public JsonElement.ArrayEnumerator GetDocument(string id)
        {
          var url = $"localhost:8080/Orders/{id}?keys={{_id: 0, timeOrdered: 1, timeDelivered: 1, deliveryAddress:1}}";
          var entries = Svc.Get<JsonDocument>(url);
          entries.Wait();
          return entries.Result.RootElement.EnumerateArray();
        }
    }
}
