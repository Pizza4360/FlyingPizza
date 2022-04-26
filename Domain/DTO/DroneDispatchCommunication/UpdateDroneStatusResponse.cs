using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using MongoDB.Bson;

namespace Domain.DTO.DroneDispatchCommunication;

public class UpdateDroneStatusResponse
{
    [JsonPropertyName("DroneUrl")]
    public string DroneId{get;set;}
        
    [JsonPropertyName("IsCompletedSuccessfully")]
    public bool IsCompletedSuccessfully{get;set;}
}