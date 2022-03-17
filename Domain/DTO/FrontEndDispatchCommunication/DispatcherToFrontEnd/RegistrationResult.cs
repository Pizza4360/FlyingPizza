using System;

namespace Domain.DTO.FrontEndDispatchCommunication.DispatcherToFrontEnd
{
    public class RegistrationResult
    {
        public bool Success { get; set; }

        public Guid BadgeNumber { get; set; }
    }
}