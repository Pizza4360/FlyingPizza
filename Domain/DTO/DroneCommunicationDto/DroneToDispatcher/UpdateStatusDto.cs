using Domain.Entities;

namespace Domain.DTO.DroneCommunicationDto.DroneToDispatcher
{
    public class UpdateStatusDto
    {
        public GeoLocation Location { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public override string ToString()
        {
            return $"location={Location}\tstatus={State}\tid={Id}";
        }
    }
}
