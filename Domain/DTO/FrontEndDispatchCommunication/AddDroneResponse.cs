﻿using System;
using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddDroneResponse
{
    [JsonPropertyName("Success")] public bool Success { get; set; }

    [JsonPropertyName("DroneId")] public string DroneId { get; set; }
}