using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignFleetResponse : BaseDto
{
    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("IsInitializedAndAssigned")]
    public bool IsInitializedAndAssigned { get; set; }

    [JsonPropertyName("FirstStatus")] public DroneStatus FirstStatus { get; set; }
}