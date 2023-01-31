using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.GatewayDefinitions;
using FluentAssertions;
using Moq;
using SimDrone;
using SimDrone.Controllers;
using Xunit;

namespace Tests.Back
{
    public class DroneTests
    {
        [Fact]
        public async Task deliver_delivery_should_return_delivery_response()
        {
            var testDestination = new GeoLocation
            {
                Latitude = 0.00002m,
                Longitude = 0.00002m
            };
            var mockedDroneDispatchGateway = new Mock<IDroneToDispatchGateway>();
            mockedDroneDispatchGateway.Setup(x => x.CompleteDelivery(It.IsAny<CompleteDeliveryRequest>()))
                .Returns(Task.FromResult(Constants.TestCompleteDeliveryResponse));
            mockedDroneDispatchGateway.Setup(x => x.UpdateDroneStatus(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(Constants.TestUpdateResponse));
            var mockedGate = mockedDroneDispatchGateway.Object;
            var sim = new SimDroneController();
            sim.ChangeGateway(mockedGate);
            var drone = new Drone(Constants.Entity, mockedGate,sim);
            sim.ChangeDrone(drone);
            drone.ChangeController(sim);
            var result = await drone.DeliverDelivery(new AssignDeliveryRequest
            {
                DroneId = Constants.DroneId,
                DeliveryId = Constants.TestDeliveryId,
                DeliveryLocation = testDestination
            });
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(Constants.TestAssignDeliveryResponse);
        }
        [Fact]
        public async Task deliver_delivery_should_change_destination()
        {
            var testDestination = new GeoLocation
            {
                Latitude = 0.00002m,
                Longitude = 0.00002m
            };
            var mockedDroneDispatchGateway = new Mock<IDroneToDispatchGateway>();
            mockedDroneDispatchGateway.Setup(x => x.CompleteDelivery(It.IsAny<CompleteDeliveryRequest>()))
                .Returns(Task.FromResult(Constants.TestCompleteDeliveryResponse));
            mockedDroneDispatchGateway.Setup(x => x.UpdateDroneStatus(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(Constants.TestUpdateResponse));
            var mockedGate = mockedDroneDispatchGateway.Object;
            var sim = new SimDroneController();
            sim.ChangeGateway(mockedGate);
            var drone = new Drone(Constants.Entity, mockedGate,sim);
            sim.ChangeDrone(drone);
            drone.ChangeController(sim);
            var result = await drone.DeliverDelivery(new AssignDeliveryRequest
            {
                DroneId = Constants.DroneId,
                DeliveryId = Constants.TestDeliveryId,
                DeliveryLocation = testDestination
            });
            result.Should().NotBeNull();
            drone.Destination.Should().BeEquivalentTo(Constants.Destination);

        }
    }
}