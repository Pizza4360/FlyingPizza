using System;

namespace Domain.DTO.DroneCommunicationDto.DispatcherToDrone
{
    public class InitializeDroneRegistrationDto
    {
        public Guid BadgeNumber { get; set; }

        public string IpAddress { get; set; }
    }
}
