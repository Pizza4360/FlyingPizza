using System;

namespace Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher
{
    public class AddDroneDto
    {
        public string IpAddress { get; set; }
        public Guid BadgeNumber { get; set; }
    }
}
