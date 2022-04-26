using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.Entities;
[BsonDiscriminator ]
public class CompositeDocument
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId _id{get;set;}
    
    [BsonElement("Fleet")]
    [JsonProperty("Fleet")]
    public List<DroneRecord> Fleet{get;set;}
    
    [BsonElement("Orders")]
    [JsonProperty("Orders")]
    public List<Order> Orders{get;set;}
    
    [BsonElement("DronesOrdersMap")]
    [JsonProperty("DronesOrdersMap")]
    public List<DronesOrderKey> DronesOrdersMap{get;set;}
    
    public override string ToString()
    {
        return $"{{Orders:{string.Join("\n", Orders)},Fleet:{string.Join("\n", Fleet)},DronesOrdersMap:{string.Join("\n", DronesOrdersMap)}}}";
    }
}
[BsonDiscriminator]
public class DronesOrderKey
{
    [BsonElement("DroneId")]
    [JsonProperty("DroneId")]
    public string DroneId{get;set;}
    [BsonElement("OrderId")]
    [JsonProperty("OrderId")]
    public string OrderId{get;set;}
    
    [BsonElement("DroneHasBeenNotified")]
    [JsonProperty("DroneHasBeenNotified")]
    public bool DroneHasBeenNotified{get;set;}

    private string nullString = "\"null\"";


    public override string ToString()
    {
        var s = OrderId == null ? nullString : OrderId;
        return $"{{\"DroneId\":\"{DroneId}\",\"OrderId\":\"{s}\",\"DroneHasBeenNotified\":\"{DroneHasBeenNotified}\"}}";
    } 
}