using System;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class RegistrationResult
    {
        public bool Success { get; set; }

        public Guid BadgeNumber { get; set; }
    }
}