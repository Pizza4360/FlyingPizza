using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways
{
    public class DroneToDispatchGateway : BaseGateway
    {
        public void ChangeHandler(HttpMessageHandler handler)
        {
            // Added for mocking reasons, no way around it
            HttpClient = new HttpClient(handler);
        }

        public CompleteOrderResponse CompleteOrder(string id)
            => (CompleteOrderResponse)SendMessage(
                id,
                "CompleteOrder",
                new CompleteOrderRequest
                {
                    Time = DateTime.Now,
                    OrderId = id
                });


        public Task<BaseDto> 
            PatchDroneStatus(DroneStatusUpdateRequest state)
            => Task.FromResult(SendMessage(Url,"PatchDroneStatus", state));
    }
}
