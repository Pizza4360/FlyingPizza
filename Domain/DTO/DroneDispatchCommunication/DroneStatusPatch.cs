using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class DroneStatusPatch
    {
        public GeoLocation Location { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public override string ToString()
        {
            return $"UpdateStatusDto={{location:{Location}\tState:{State}\tId={Id}}}";
        }
    }
}
