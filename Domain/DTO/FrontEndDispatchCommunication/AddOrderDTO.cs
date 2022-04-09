using Domain.Entities;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddOrderDTO
    {
        public string DroneId;
        public GeoLocation DeliveryLocaion;
        public override string ToString() => $"{{Id:{DroneId},DeliveryLocation:{DeliveryLocaion}}}";
    }
}