using System;
using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignFleetRequest : BaseDto
        {
            [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
            public Guid BadgeNumber { get; set; }           
            
            [BsonElement("DispatcherUrl"), JsonPropertyName("DispatcherUrl")]
            public string DispatcherUrl;
            
            [BsonElement("HomeLocation"), JsonPropertyName("HomeLocation")]
            public GeoLocation HomeLocation { get; set; }
            public override string ToString() => $"{{BadgeNumber:{BadgeNumber},DispatcherUrl:{DispatcherUrl},HomeLocation:{HomeLocation}}}";
        }
}