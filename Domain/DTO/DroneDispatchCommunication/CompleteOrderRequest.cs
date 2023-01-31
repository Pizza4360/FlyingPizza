using System;
using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteDeliveryRequest : BaseDto
{
    [JsonPropertyName("DeliveryId")] public string DeliveryId { get; set; }

    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("Time")] public DateTime Time { get; set; }
}