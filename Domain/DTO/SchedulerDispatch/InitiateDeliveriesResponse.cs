using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;

namespace Domain.DTO.SchedulerDispatch;

public class InitiateDeliveriesResponse
{
    [JsonPropertyName("Requests")] public List<EnqueueOrderResponse> Responses { get; set; }
}