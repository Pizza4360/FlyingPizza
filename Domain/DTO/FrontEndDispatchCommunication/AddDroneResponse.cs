using System;
using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddDroneResponse
{
    [JsonPropertyName("Success")] public bool Success { get; set; }

    [JsonPropertyName("BadgeNumber")] public Guid BadgeNumber { get; set; }
}