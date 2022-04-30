using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationRequest
{
    [JsonPropertyName("Record")] public DroneRecord Record { get; set; }

    [JsonPropertyName("DispatchIpAddress")]
    public string DispatchIpAddress { get; set; }
}