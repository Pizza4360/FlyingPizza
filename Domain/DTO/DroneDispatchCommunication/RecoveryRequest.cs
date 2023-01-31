using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class RecoveryRequest : BaseDto
{
    public DroneEntity Entity { get; set; } 
}