using System.Text.Json.Serialization;
using MongoDB.Entities;
using Order = Domain.Entities.Order;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class EnqueueOrderRequest
   
{
    [JsonPropertyName("OrderId")]
    public Order Order { get; set; }
}