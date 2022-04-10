using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class DroneRegistrationInfo : BaseDTO
    {
        [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
        public int BadgeNumber { get; set; }
        
        [BsonElement("IpAddress"), JsonPropertyName("IpAddress")]
        public string IpAddress { get; set; }        public override string ToString() => $"{{BadgeNumber:{BadgeNumber},IpAddress:{IpAddress}}}";

        //TODO: may not be included later on, just added since I found dispatcher needs these
        // public GeoLocation HomeLocation { get; set; }
        // public string DispatcherUrl { get; set; }
    }
}
