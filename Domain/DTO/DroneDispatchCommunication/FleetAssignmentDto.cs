using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class FleetAssignmentDto
        {
            public int BadgeNumber;
            public string DispatcherUrl;
            public GeoLocation HomeLocation;
        }
}