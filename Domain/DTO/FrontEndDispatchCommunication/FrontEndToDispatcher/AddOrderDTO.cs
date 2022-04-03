using Domain.Entities;

namespace Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher
{
    public class AddOrderDTO
    {
        public string Id;
        public GeoLocation DeliveryLocaion;
        public override string ToString() => $"AddOrderDto:{{Id:{Id},DeliveryLocation:{DeliveryLocaion}}}";
    }
}