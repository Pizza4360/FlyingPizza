using System;

namespace Domain.DTO.DispatcherFrontEnd.DispatcherToFrontEnd
{
    public class ResponseRegistrationDto
    {
        public bool Success { get; set; }

        public Guid BadgeNumber { get; set; }
    }
}