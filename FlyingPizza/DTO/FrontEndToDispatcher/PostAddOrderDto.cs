
using FlyingPizza;
using FlyingPizza.Drone;

namespace FlyingPizza.DTO.DispatcherFrontEnd
{
    public class PostAddOrderDto
    {
        public string Id;
        public GeoLocation DeliveryLocaion;
        public override string ToString() => $"AddOrderDto:{{Id:{Id},DeliveryLocation:{DeliveryLocaion}}}";

        public static PostAddOrderDto From(Order custOrder)
        {
            return new PostAddOrderDto
            {
                DeliveryLocaion = custOrder.DeliveryLocation,
                Id = custOrder.Id
            };
        }
    }
}