using System;

namespace Domain.DTO.FrontEndDispatchCommunnication
{
    public class AddDrone
    {
        public string IpAddress { get; set; }
        
        public Guid BadgeNumber { get; set; }
    }
}
