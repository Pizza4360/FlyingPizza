using System.Data;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Domain;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignFleetResponse
    {
        // [JsonPropertyName("DroneId")]
        public string DroneId;

        // [JsonPropertyName("IsInitializedAndAssigned")]
        public bool IsInitializedAndAssigned;
        
        // [JsonPropertyName("FirstState")]
        public DroneState FirstState { get; set; }
    }
}
