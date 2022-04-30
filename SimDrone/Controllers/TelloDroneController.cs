/*
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.InterfaceImplementations.Gateways;
using Microsoft.AspNetCore.Mvc;

namespace SimDrone.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelloDroneController : SimDroneController
    {
        private TelloDrone _drone;
        private static DroneToDispatchGateway _gateway;

        /// <summary>
        /// Command a drone to deliver an order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost("deliver")]
        public async Task<string> DeliverOrder(
        AssignDeliveryRequest order)
        {
            Console.WriteLine($"Delivering {order}");
            return _drone.DeliverOrder(order.OrderLocation).ToString();
        }

        
        [HttpPost("InitDrone")]
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
                Id = initInfo.DroneId,
                Okay = okay
            });
        }
        
        [HttpPost("StartDrone")]
        public Task<OkObjectResult> StartDrone(
        TelloDrone drone)
        {
            _drone = drone;
            return Task.FromResult(Ok("hello, world"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("AssignFleet")]
        public async Task<CompleteRegistrationResponse> AssignToFleet(
        CompleteRegistrationRequest post)
        {
            Console.WriteLine("Generating simulated drone...");
            _drone = new TelloDrone(post.Record, new DroneToDispatchGateway
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

        public TelloDroneController(TelloDrone drone)
        {
            _drone = drone;
        }
    }
}
*/

