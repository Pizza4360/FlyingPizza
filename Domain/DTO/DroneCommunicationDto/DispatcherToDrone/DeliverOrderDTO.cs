using System.Collections.Generic;
using Domain.Entities;

namespace Domain.DTO.DroneCommunicationDto.DispatcherToDrone
{
    public class DeliverOrderDto
    {
        public string OrderId { get; set; }

        public List<GeoLocation> Route { get; set; }
    }
}
