using Domain.Entities;

namespace Domain.DTO.DispatcherDrone.DroneToDispatcher
{
    public class PutStatusDto
    {
        public GeoLocation Location { get; set; }
        public string State { get; set; }
        public string BadgeNumber { get; set; }
        public override string ToString()
        {
            return $"UpdateStatusDto={{location:{Location}\tState:{State}\tId={BadgeNumber}}}";
        }
    }
}
