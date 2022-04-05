using Domain.Entities;

namespace Domain.DTO.DispatcherFrontEnd.FrontEndToDispatcher
{
    public class PostAddOrderDto
    {
        public string Id;
        public GeoLocation DeliveryLocaion;
        public override string ToString() => $"AddOrderDto:{{Id:{Id},DeliveryLocation:{DeliveryLocaion}}}";

        public static PostAddOrderDto From(Order custOrder)
        {
            throw new System.NotImplementedException();
        }
    }
}