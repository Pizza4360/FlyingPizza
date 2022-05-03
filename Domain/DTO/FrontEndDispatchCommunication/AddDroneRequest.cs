﻿using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddDroneRequest
{
    [JsonPropertyName("DroneId")] public string DroneId { get; set; }
    [JsonPropertyName("DroneUrl")] public string DroneUrl { get; set; }
}