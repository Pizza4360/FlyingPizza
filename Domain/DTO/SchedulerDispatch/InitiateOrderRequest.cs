using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;

namespace Domain.DTO.SchedulerDispatch;

public class InitiateDeliveriesRequest
{
    [JsonPropertyName("Requests")] public List<EnqueueOrderRequest> Requests { get; set; }
}