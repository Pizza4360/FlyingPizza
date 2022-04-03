using System;

namespace Domain.DTO.DispatcherFrontEnd.FrontEndToDispatcher{
    public class GetRegistrationResultsDto
    {
        public string IpAddress { get; set; }
        public Guid BadgeNumber { get; set; }
    }
}
