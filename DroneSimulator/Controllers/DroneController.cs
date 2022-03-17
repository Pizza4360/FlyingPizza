using System;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DroneSimulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DroneController : ControllerBase
    {
        private readonly IDronesRepository _dronesRepository;
        private readonly IDispatcherGateway _dispatcherGateway;
        private readonly Drone _drone;

        public DroneController(IDronesRepository dronesRepository, IDispatcherGateway dispatcherGateway)
        {
            _dronesRepository = dronesRepository;
            _dispatcherGateway = dispatcherGateway;
            _drone = new Drone(123, new GeoLocation(), dispatcherGateway); // TODO: initialize this from the drones repository, based on a drone id from environment variables
            Console.WriteLine(_drone);
        }

        [HttpPost("assigndelivery")]
        public async Task<IActionResult> AssignDelivery(DeliverOrderDto order)
        {
            _drone.deliverOrder(order.Route[0]);
            return Ok();
        }

        [HttpPost("initregistration")]
        public async Task<IActionResult> InitializeRegistration()
        {
            Console.WriteLine("YAY");
            return Ok("YAY");
        }

        [HttpPost("completeregistration")]
        public async Task<IActionResult> CompleteRegistration()
        {
            return Ok();
        }
    }
}
