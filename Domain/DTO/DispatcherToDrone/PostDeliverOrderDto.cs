using Domain.Entities;

namespace Domain.DTO.DispatcherToDrone
{
    public class PostDeliverOrderDto
    {
        public string OrderId { get; set; }

        public GeoLocation OrderLocation { get; set; }
    }
}
