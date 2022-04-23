using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddOrderRequest
   
{
    [JsonPropertyName("OrderId")]
    public string OrderId { get; set; }
    [JsonPropertyName("OrderLocation")]
    public GeoLocation OrderLocation { get; set; }
}