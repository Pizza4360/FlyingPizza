// See https://aka.ms/new-console-template for more information


using System.Text.Json.Nodes;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


Console.WriteLine("Hello, World!");

/*
var mongoClient = new MongoClient("mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
var mongoDatabase = mongoClient.GetDatabase("Capstone");
var _collection = mongoDatabase.GetCollection<CompositeDocument>("CompositeTest");
var unfulfilledOrders = _collection.Find(x => true).FirstOrDefault();
// Console.WriteLine(unfulfilledOrders);
*/




// var drones = repo.GetDrones().Result;
// Console.WriteLine(drones);
var dbString = @"
{
  ""_id"": {
    ""$oid"": ""62676d0fe90cbf27acbc9cdd""
  },
  ""Orders"": [
    {
      ""OrderId"": ""2345678901"",
      ""Items"": [
        {
          ""type"": ""pizza"",
          ""toppings"": [
            ""artichoke"",
            ""pineapple"",
            ""olives""
          ],
          ""size"": ""medium"",
          ""price"": 13.42
        },
        {
          ""type"": ""pizza"",
          ""toppings"": [
            ""tomatoes"",
            ""jelly beans""
          ],
          ""size"": ""large"",
          ""price"": 13.42
        }
      ],
      ""CustomerName"": ""harrison"",
      ""DeliveryAddress"": ""1201 5th St, Denver, CO 80204"",
      ""DeliveryLocation"": {
        ""Latitude"": ""39.74255824419709"",
        ""Longitude"": ""-105.0092770462987""
      },
      ""TimeOrdered"": {
        ""$date"": ""2019-12-22T23:48:07.730Z""
      },
      ""TimeDelivered"": {
        ""$date"": ""2022-04-09T05:59:50.682Z""
      },
      ""HasBeenDelivered"": true
    },
    {
      ""OrderId"": ""4567890123"",
      ""Items"": [
        {
          ""type"": ""pizza"",
          ""toppings"": [
            ""artichoke"",
            ""pineapple"",
            ""olives""
          ],
          ""size"": ""medium"",
          ""price"": 13.42
        },
        {
          ""type"": ""pizza"",
          ""toppings"": [
            ""tomatoes"",
            ""jelly beans""
          ],
          ""size"": ""large"",
          ""price"": 13.42
        }
      ],
      ""CustomerName"": ""harrison"",
      ""DeliveryAddress"": ""1201 5th St, Denver, CO 80204"",
      ""DeliveryLocation"": {
        ""Latitude"": ""39.74255824419709"",
        ""Longitude"": ""-105.0092770462987""
      },
      ""TimeOrdered"": {
        ""$date"": ""2019-12-22T23:48:07.730Z""
      },
      ""TimeDelivered"": {
        ""$date"": ""2022-04-09T05:59:50.682Z""
      },
      ""HasBeenDelivered"": true
    }
  ],
  ""Fleet"": [
    {
      ""DroneId"": ""6266e168be40496aa38f9dc3"",
      ""Orders"": [{
      ""OrderId"": ""555555555555"",
      ""Items"": [
        {
          ""type"": ""pizza"",
          ""toppings"": [
            ""artichoke"",
            ""pineapple"",
            ""olives""
          ],
          ""size"": ""medium"",
          ""price"": 13.42
        },
        {
          ""type"": ""pizza"",
          ""toppings"": [
            ""tomatoes"",
            ""jelly beans""
          ],
          ""size"": ""large"",
          ""price"": 13.42
        }
      ],
      ""CustomerName"": ""harrison"",
      ""DeliveryAddress"": ""1201 5th St, Denver, CO 80204"",
      ""DeliveryLocation"": {
        ""Latitude"": ""39.74255824419709"",
        ""Longitude"": ""-105.0092770462987""
      },
      ""TimeOrdered"": {
        ""$date"": ""2019-12-22T23:48:07.730Z""
      },
      ""TimeDelivered"": {
        ""$date"": ""2022-04-09T05:59:50.682Z""
      },
      ""HasBeenDelivered"": true
    }],
      ""BadgeNumber"": {
        ""$binary"": {
          ""base64"": ""9J79BbUFMU+9Pwb37bCFwg=="",
          ""subType"": ""03""
        }
      },
      ""Destination"": {
        ""Latitude"": ""39.74386695629378"",
        ""Longitude"": ""-105.00610500179027""
      },
      ""CurrentLocation"": {
        ""Latitude"": ""39.74386695629378"",
        ""Longitude"": ""-105.00610500179027""
      },
      ""HomeLocation"": {
        ""Latitude"": ""39.74386695629378"",
        ""Longitude"": ""-105.00610500179027""
      },
      ""State"": 0,
      ""DroneUrl"": ""http://localhost:85"",
      ""DispatchUrl"": ""http://localhost:83""
    }
  ]
}
";

var items_sring = @"
   [
  {
    ""type"": ""pizza"",
    ""toppings"": [
    ""artichoke"",
    ""pineapple"",
    ""olives""
      ],
    ""size"": ""medium"",
    ""price"": 13.42
  },
  {
    ""type"": ""pizza"",
    ""toppings"": [
    ""tomatoes"",
    ""jelly beans""
      ],
    ""size"": ""large"",
    ""price"": 13.42
  }
]
";
// var bson = JsonObject.Parse(items_sring).ToBsonDocument();

var mongoClient = new MongoClient("mongodb+srv://capstone:Ms2KqQKc5U3gFydE@cluster0.rjlgf.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
var mongoDatabase = mongoClient.GetDatabase("Capstone");
var _collection = mongoDatabase.GetCollection<CompositeDocument>("CompositeTest");

// var _compositeDocument = BsonSerializer.Deserialize<CompositeDocument>(bson);
// _compositeDocument._id = ObjectId.GenerateNewId();

MongoDB.Driver.FindOneAndReplaceOptions<Domain.Entities.CompositeDocument, Domain.Entities.CompositeDocument> options =
  new FindOneAndReplaceOptions<CompositeDocument>();
options.IsUpsert = true;
System.Threading.CancellationToken token = CancellationToken.None;
CompositeRepository repo = new CompositeRepository();
Console.WriteLine(repo);

var _id = BsonObjectId.Create("62676d0fe90cbf27acbc9cdd");
var filter = Builders<CompositeDocument>.Filter.Eq("_id", _id);

var update = Builders<CompositeDocument>
            .Update
            .Set("Fleet", new List<DroneRecord>{new DroneRecord{BadgeNumber = Guid.NewGuid(),Orders = new List<Order>
             {
               new Order
               {
                 CustomerName = "SMOOTH",
                 DeliveryAddress = "blah",
                 TimeOrdered = DateTime.Now
               }
             }}});
            // .Set("Orders", _compositeDocument.Orders); 
var result = _collection.UpdateOne(filter, update, new UpdateOptions{IsUpsert = true});


// BsonArray fleetArray = new BsonArray();
// foreach (var term in _compositeDocument.Fleet)
// {
//   fleetArray.Add(term.ToBsonDocument());
// }
// BsonArray orderArray = new BsonArray();
// foreach (var term in _compositeDocument.Orders)
// {
//   orderArray.Add(term.ToBsonDocument());
// }
   

