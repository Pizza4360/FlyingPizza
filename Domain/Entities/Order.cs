using System;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Order : IBaseEntity
    {
        public string Id { get; set; }

        public string CustomerName { get; set; }

        public string DeliveryAddress { get; set; }

        public GeoLocation DeliveryLocation { get; set; }

        public DateTimeOffset TimeOrdered { get; set; }

        public DateTimeOffset? TimeDelivered { get; set; }

        public bool HasBeenDelivered
        { get { return (TimeDelivered != null); } }
    }
}
