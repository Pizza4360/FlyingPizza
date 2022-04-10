using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.Gateways;
using Microsoft.AspNetCore.Mvc;

namespace SimDrone.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimDroneController : ControllerBase
    {
        private Drone _drone;
        private DroneToDispatchGateway _gateway;
        private int _badgeNumber;


        /// <summary>
        /// Command a drone to deliver an order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost("deliver")]
        public async Task<IActionResult> DeliverOrder(Delivery order)
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
        // Step 3, drone saves the incoming DroneToDispatherGateway then uses it
        // to give initial state and location back
        [HttpPost("initregistration")]
        public async Task<bool> InitializeRegistration(InitGatewayPost initInfo)
        {
            Console.WriteLine();
            _badgeNumber = initInfo.BadgeNumber;
            _gateway = initInfo.Gateway;
            if (_gateway.PostDroneStatus(0,0,"Ready"))
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// For testing purposes.
        /// </summary>
        /// <returns>"hello, world"</returns>
        // Step 7, receive the BadgeNumberAndHome through a drone object
        [HttpPost("complete_registration")]
        public async Task<OkObjectResult> CompleteRegistration(SimDrone.Drone drone)
        {
            _drone = drone;
            return Ok("hello, world");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("completeregistration")]
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

namespace Domain.DTO.DroneDispatchCommunication
{

    public class CompleteRegistrationPost
    {
       public DroneRecord Record { get; set; }
       public DroneToDispatchGateway Gateway { get; set; }
    }
    public class OkToCompletePost
    {
        public Task<HttpContent> Message { get; set; }
        public bool DoComplete { get; set; }
    }
}