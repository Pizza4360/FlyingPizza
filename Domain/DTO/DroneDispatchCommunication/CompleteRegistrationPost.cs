using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationRequest : BaseDTO
{
    public DroneRecord Record { get; set; }
    public string DispatchIpAddress { get; set; }
}