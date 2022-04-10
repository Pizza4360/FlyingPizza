using System.Text.Json.Serialization;
using Domain.DTO.Shared;
using Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignFleetRequest : BaseDTO
        {
            [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
            public int BadgeNumber { get; set; }           
            
            [BsonElement("DispatcherUrl"), JsonPropertyName("DispatcherUrl")]
            public string DispatcherUrl;
            
            [BsonElement("HomeLocation"), JsonPropertyName("HomeLocation")]
            public GeoLocation HomeLocation { get; set; }
            public override string ToString() => $"{{BadgeNumber:{BadgeNumber},DispatcherUrl:{DispatcherUrl},HomeLocation:{HomeLocation}}}";
        }
}