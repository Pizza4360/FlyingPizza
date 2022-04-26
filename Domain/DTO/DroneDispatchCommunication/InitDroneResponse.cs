using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using MongoDB.Bson;

namespace Domain.DTO.DroneDispatchCommunication;

public class InitDroneResponse
{ 
    public string DroneId { get; set; }
    public bool Okay { get; set; }
}