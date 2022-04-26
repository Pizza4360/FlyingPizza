using System.Text.Json.Serialization;
using MongoDB.Driver;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteOrderResponse
{
    public bool IsAcknowledged { get; set; }
    public UpdateResult UpdateResult{get;set;}
}
