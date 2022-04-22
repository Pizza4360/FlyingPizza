using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationRequest: BaseDto
{
    [JsonPropertyName("Record")]
    public DroneRecord Record { get; set; }
    
    [JsonPropertyName("DispatchIpAddress")]
    public string DispatchIpAddress { get; set; }
}
