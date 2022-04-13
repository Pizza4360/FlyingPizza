using System;
using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignFleetRequest : BaseDTO
        {
            [BsonId]
            [BsonElement("Id")]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("DroneIp"), JsonPropertyName("DroneIp")]
            public string DroneIp { get; set; }
            
            [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
            [BsonId(IdGenerator = typeof(CombGuidGenerator))]
            public Guid BadgeNumber { get; set; }           
            
            [BsonElement("HomeLocation"), JsonPropertyName("HomeLocation")]
            public GeoLocation HomeLocation { get; set; }
            public override string ToString() => $"{{BadgeNumber:{BadgeNumber},DispatcherUrl:{DispatcherIp},HomeLocation:{HomeLocation}}}";
            
            [BsonElement("DispatcherUrl"), JsonPropertyName("DispatcherUrl")]
            public string DispatcherIp;
        }
}