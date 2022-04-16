using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationPost : BaseDto
{
    public DroneRecord Record { get; set; }
    public DroneToDispatchGateway Gateway { get; set; }
}