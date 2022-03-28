using Domain.Entities;

namespace Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher
{
    public class AddOrderDTO
    {
        public string Id;
        public GeoLocation DeliveryLocaion;
    }
}