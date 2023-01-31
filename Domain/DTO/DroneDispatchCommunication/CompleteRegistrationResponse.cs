using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationResponse : BaseDto
{
    [JsonPropertyName("Model")] public DroneEntity Entity { get; set; }

    [JsonPropertyName("Okay")] public bool Okay { get; set; }
}