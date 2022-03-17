using System;

namespace Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher
{
    public class AddDrone
    {
        public string IpAddress { get; set; }
        
        public Guid BadgeNumber { get; set; }
    }
}
