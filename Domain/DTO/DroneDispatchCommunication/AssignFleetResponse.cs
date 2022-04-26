using System.Data;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Domain;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignFleetResponse
    {
        public string DroneId{get;set; }
        public bool IsInitializedAndAssigned{get;set;}
        public DroneState FirstState { get; set; }
    }
}
