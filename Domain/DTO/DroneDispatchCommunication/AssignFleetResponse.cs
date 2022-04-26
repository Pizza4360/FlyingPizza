using System.Data;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Domain;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignFleetResponse
    {
        [JsonPropertyName("DroneUrl")]
        public string DroneId{get;set; }

        [JsonPropertyName("IsInitializedAndAssigned")]
        public bool IsInitializedAndAssigned{get;set;}

        [JsonPropertyName("FirstState")]
        public DroneState FirstState { get; set; }
    }
}
