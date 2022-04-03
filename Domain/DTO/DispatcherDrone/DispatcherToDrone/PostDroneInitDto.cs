using System;

namespace Domain.DTO.DispatcherDrone.DispatcherToDrone
{
    public class PostDroneInitDto
    {
        public Guid BadgeNumber { get; set; }

        public string IpAddress { get; set; }
    }
}
