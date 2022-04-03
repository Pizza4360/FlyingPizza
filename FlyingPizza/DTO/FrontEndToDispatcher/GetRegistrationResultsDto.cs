using System;

namespace FlyingPizza.DTO.DispatcherFrontEnd
{ 
    public class GetRegistrationResultsDto
    {
        public string IpAddress { get; set; }
        public Guid BadgeNumber { get; set; }
    }
}
