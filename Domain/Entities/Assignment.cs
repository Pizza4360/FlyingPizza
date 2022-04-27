using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
namespace Domain.Entities;

public class Assignment : BaseEntity
{
    [BsonElement("DroneId")]
    [JsonProperty("DroneId")]
    public string DroneId{get;set;}
    [BsonElement("OrderId")]
    [JsonProperty("OrderId")]
    public string OrderId{get;set;}
    
    [BsonElement("ShouldNotifyDrone")]
    [JsonProperty("ShouldNotifyDrone")]
    public bool ShouldNotifyDrone{get;set;}

    public override string ToString()
    {
        var s = OrderId == null ? "\"null\"" : OrderId;
        return $"{{\"DroneId\":\"{DroneId}\",\"OrderId\":\"{s}\",\"ShouldNotifyDrone\":\"{ShouldNotifyDrone}\"}}";
    } 
}
