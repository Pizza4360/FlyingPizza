using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class EnqueueDeliveryRequest : BaseDto

{
    [JsonPropertyName("DeliveryId")] public string DeliveryId { get; set; }

    [JsonPropertyName("DeliveryLocation")] public GeoLocation DeliveryLocation { get; set; }

    public void Deconstruct(out object drone, out object delivery)
    {
        (drone, delivery) = (DeliveryId, DeliveryLocation);
    }
}