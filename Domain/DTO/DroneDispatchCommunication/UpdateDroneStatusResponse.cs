using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.DTO.DroneDispatchCommunication;

public class UpdateDroneStatusResponse
{
    public string DroneId{get;set;}
    public bool IsCompletedSuccessfully{get;set;}
    // public UpdateResult UpdateResult{get;set;}
}