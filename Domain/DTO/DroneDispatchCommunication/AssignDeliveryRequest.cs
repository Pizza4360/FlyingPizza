using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignDeliveryRequest : BaseDto
{
    [JsonPropertyName("DeliveryId")] public string DeliveryId { get; set; }

    [JsonPropertyName("DeliveryLocation")] public GeoLocation DeliveryLocation { get; set; }

    [JsonPropertyName("DroneId")] public string DroneId { get; set; }
}