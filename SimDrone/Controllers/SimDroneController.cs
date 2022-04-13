using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.InterfaceImplementations.Gateways;
using Microsoft.AspNetCore.Mvc;

namespace SimDrone.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimDroneController : ControllerBase
    {
        private Drone _drone;
        private static DroneToDispatchGateway _gateway;

        [HttpPost("InitializeRegistration")]
        public Task<InitDroneResponse> InitializeRegistration(
        InitDroneRequest initInfo)
        {
            Console.WriteLine(initInfo.ToJsonString());
            var okay = _drone.Id == null;
            if(okay)
            {
                _gateway = new DroneToDispatchGateway{ Url = initInfo.DroneIp };
            }
            return Task.FromResult(new InitDroneResponse
            {
                Id = initInfo.Id,
                Okay = okay
            });
        }
        
        
        [HttpPost("CompleteRegistration")]
        public async Task<CompleteRegistrationResponse> AssignToFleet(
        CompleteRegistrationRequest post)
        {
            Console.WriteLine("Generating simulated drone...");
            _drone = new Drone(post.Record, new DroneToDispatchGateway
            {
                Url = post.DispatchIpAddress
            });
            var doneString
                = $"SimDrone successfully initialized.\nDrone -->{_drone}";
            Console.WriteLine(doneString);
            return new CompleteRegistrationResponse
            {
                Record = post.Record,
                Okay = true
            };
        }

        
        [HttpPost("AssignDelivery")]
        public async Task<string> DeliverOrder(
        AssignDeliveryRequest order)
        {
            Console.WriteLine($"Delivering {order}");
            return _drone.DeliverOrder(order.OrderLocation)
                .ToString();
        }
    }
}
