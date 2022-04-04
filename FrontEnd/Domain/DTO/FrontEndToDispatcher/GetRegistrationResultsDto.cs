using System;

namespace FlyingPizza.Domain.DTO.FrontEndToDispatcher
{ 
    public class GetRegistrationResultsDto
    {
        public string IpAddress { get; set; }
        public Guid BadgeNumber { get; set; }
    }
}
