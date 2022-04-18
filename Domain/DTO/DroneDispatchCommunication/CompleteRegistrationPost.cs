using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationRequest : BaseDto
{
    public DroneRecord Record { get; set; }
    public string DispatchIpAddress { get; set; }
}
