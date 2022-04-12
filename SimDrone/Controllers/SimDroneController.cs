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

        /// <summary>
        /// Command a drone to deliver an order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost("deliver")]
        public async Task<IActionResult> DeliverOrder(AssignDeliveryRequest order)
        {
            Console.WriteLine($"Delivering {order}");
            _drone.DeliverOrder(order.OrderLocation);
            return Ok();
        }
        
        /// <summary>
        /// This method is called when a drone has been pinged to be
        /// initialized into a fleet. SimDroneController is idle until
        /// this method is called.
        /// </summary>
        /// <returns></returns>
        // Step 3, drone saves the incoming url in a new
        // DroneToDispatcherGateway object, then uses it
        // to give initial state and location back
        [HttpPost("InitializeRegistration")]
        public async Task<string?> InitializeRegistration(InitDroneRequest initInfo)
        {
            _gateway = new DroneToDispatchGateway
            {
                Url = initInfo.Url
            };
            Console.WriteLine();
            var _badgeNumber = initInfo.BadgeNumber;
            return await _gateway.PostInitialStatus(0, 0, "Ready") ;
        }
        
        /// <summary>
        /// For testing purposes.
        /// </summary>
        /// <returns>"hello, world"</returns>
        // Step 7, receive the BadgeNumberAndHome through a drone object
        [HttpPost("StartDrone")]
        public Task<OkObjectResult> StartDrone(Drone drone)
        {
            _drone = drone;
            return Task.FromResult(Ok("hello, world"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("CompleteRegistration")]
        public async Task<IActionResult> CompleteRegistration(
        CompleteRegistrationPost post)
        {
            Console.WriteLine("Generating simulated drone...");
            _drone = new Drone(post.Record, post.Gateway);
            // TODO: initialize this from the drones repository, based on a drone id from environment variables
            var doneString
                = $"SimDrone successfully initialized.\nDrone -->{_drone}";
            Console.WriteLine(doneString);
            return Ok(doneString);
        }
    }
}