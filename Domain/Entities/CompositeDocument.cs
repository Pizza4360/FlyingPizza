using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class CompositeDocument
{
    [BsonElement("Orders")]
    public List<Order> Orders{get;set;}
    [BsonId]
    public ObjectId _id{get;set;}
    public override string ToString()
    {
        return string.Join("\n", Orders);
    }

    [BsonElement("Fleet")]
    public List<DroneRecord> Fleet{get;set;}
}