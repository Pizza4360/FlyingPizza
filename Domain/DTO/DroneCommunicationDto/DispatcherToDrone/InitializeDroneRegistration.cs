using System;

namespace Domain.DTO.DroneCommunicationDto.DispatcherToDrone
{
    public class InitializeDroneRegistration
    {
        public Guid BadgeNumber { get; set; }

        public string IpAddress { get; set; }
    }
}
