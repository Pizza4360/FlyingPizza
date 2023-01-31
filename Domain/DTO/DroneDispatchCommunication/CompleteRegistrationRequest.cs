using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationRequest : BaseDto
{
    [JsonPropertyName("Model")] public DroneEntity Entity { get; set; }
}