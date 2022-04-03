using Domain.Entities;

namespace Domain.DTO.DispatcherDrone.DispatcherToDrone
{
    public class PostDeliverOrderDto
    {
        public string OrderId { get; set; }

        public GeoLocation OrderLocation { get; set; }
    }
}
