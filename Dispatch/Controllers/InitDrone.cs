using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;

namespace Dispatch.Controllers;

public class InitDrone
{
    public DroneRecord Record { get; set; }
    public DroneStatusUpdateRequest FistStatusUpdateRequestUpdate { get; set; }
    public override string ToString() => $"{{Record: {Record},FistStatusUpdateRequestUpdate:{FistStatusUpdateRequestUpdate}}}";

}