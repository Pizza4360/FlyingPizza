using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class Delivery
    {
        public string OrderId { get; set; }

        public GeoLocation OrderLocation { get; set; }
    }
}
