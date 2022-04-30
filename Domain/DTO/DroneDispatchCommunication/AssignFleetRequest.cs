using System;
using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignFleetRequest
{
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }

    [JsonPropertyName("DroneUrl")]
    public string DroneIp {get;set;}
            
    [JsonPropertyName("DispatchUrl")]
    public string DispatchIp{get;set;}

    [JsonPropertyName("HomeLocation")]
    public GeoLocation HomeLocation { get; set; }
}
