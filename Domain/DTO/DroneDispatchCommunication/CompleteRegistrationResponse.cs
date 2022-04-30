using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationResponse
{
    [JsonPropertyName("Record")] public DroneRecord Record { get; set; }

    [JsonPropertyName("Okay")] public bool Okay { get; set; }
}