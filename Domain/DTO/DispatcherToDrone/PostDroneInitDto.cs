using System;

namespace Domain.DTO.DispatcherToDrone
{
    public class PostDroneInitDto
    {
        public Guid BadgeNumber { get; set; }

        public string IpAddress { get; set; }
    }
}
