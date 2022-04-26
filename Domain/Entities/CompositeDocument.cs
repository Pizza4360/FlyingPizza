using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.Entities;

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
    public override string ToString()
    {
        return $"{{Orders:{string.Join("\n", Orders)},Fleet:{string.Join("\n", Fleet)}}}";
    }
    
}