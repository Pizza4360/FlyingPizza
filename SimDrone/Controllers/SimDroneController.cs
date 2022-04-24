using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
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

        [HttpPost("InitDrone")]
        public InitDroneResponse InitDrone(
        InitDroneRequest initInfo)
        {
            Console.WriteLine(initInfo.ToJsonString());
            var okay = _drone.Id == null;
            if(okay)
            {
                _gateway = new DroneToDispatchGateway{ Url = initInfo.DroneIp };
            }
            return new InitDroneResponse
            {
                Id = initInfo.DroneId,
                Okay = okay
            };
        }
        
        
        [HttpPost("AssignFleet")]
        public AssignFleetResponse AssignFleet(
        AssignFleetRequest request)
        {
            Console.WriteLine("Generating simulated drone...");
            _gateway = new DroneToDispatchGateway
            {
                Url = request.DispatcherIp
            };
            _drone = new Drone(new DroneRecord
            {
                BadgeNumber = request.BadgeNumber,
                CurrentLocation = request.HomeLocation,
                Destination = request.HomeLocation,
                DispatcherUrl = request.DispatcherIp,
                DroneIp = request.DispatcherIp,
                HomeLocation = request.HomeLocation,
                Id = request.DroneId,
                OrderId = null
            }, _gateway);
            var doneString
                = $"SimDrone successfully initialized.\nDrone -->{_drone}";
            Console.WriteLine(doneString);
            return new AssignFleetResponse
            {
                FirstState = DroneState.Ready,
                Id = request.DroneId,
                IsInitializedAndAssigned = true
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
