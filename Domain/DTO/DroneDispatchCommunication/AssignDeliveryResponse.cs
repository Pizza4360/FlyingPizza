using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using MongoDB.Bson;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignDeliveryResponse
{
    public string DroneId{get;set;}
    public bool Success{get;set;}
}