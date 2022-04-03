using System;
using Domain.Entities;

namespace Domain.DTO.DispatcherDrone.DispatcherToDrone
{
    public class PostDroneRegistrationDto
    {
        public Guid BadgeNumber { get; set; }

        public string IpAddress { get; set; }
        
        //TODO: may not be included later on, just added since I found dispatcher needs these
        // public GeoLocation HomeLocation { get; set; }
        // public string DispatcherUrl { get; set; }
    }
}
