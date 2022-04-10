using Domain.Gateways;
namespace Domain.DTO.DroneDispatchCommunication
{
    public class InitGatewayPost
    {
        public DroneToDispatchGateway Gateway { get; set; }
        public string Url { get; set; }
        public int BadgeNumber { get; set; }
    }
}
