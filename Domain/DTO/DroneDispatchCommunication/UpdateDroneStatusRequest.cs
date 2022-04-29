using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;
//
// public class UpdateDroneStatusRequest
// {
//     [JsonPropertyName("DroneId")]
//     public string DroneId{get;set;}
//
//     [JsonPropertyName("Location")]
//     public GeoLocation Location { get; set; } 
//         
//     [JsonPropertyName("State")]
//     public DroneState State { get; set; }
// }