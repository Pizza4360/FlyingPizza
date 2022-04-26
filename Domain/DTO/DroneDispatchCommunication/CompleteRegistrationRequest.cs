using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationRequest
{
    public DroneRecord Record { get; set; }
    public string DispatchIpAddress { get; set; }
}
