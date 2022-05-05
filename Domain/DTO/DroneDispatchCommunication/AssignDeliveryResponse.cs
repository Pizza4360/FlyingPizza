using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignDeliveryResponse
{
    [JsonPropertyName("OrderId")] public string OrderId { get; set; }

    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("Success")] public bool WillDeliverOrder { get; set; }
}