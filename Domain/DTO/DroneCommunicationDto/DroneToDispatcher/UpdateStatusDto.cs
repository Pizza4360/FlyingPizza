using Domain.Entities;

namespace Domain.DTO.DroneCommunicationDto.DroneToDispatcher
{
    public class UpdateStatusDto
    {
        public GeoLocation Location { get; set; }
        public string Status { get; set; }
    }
}
