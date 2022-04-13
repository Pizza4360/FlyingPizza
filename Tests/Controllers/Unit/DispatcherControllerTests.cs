using System.Threading.Tasks;
using Dispatch.Controllers;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceImplementations.Gateways;
using Domain.InterfaceImplementations.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Controllers.Unit
{


    public class DispatcherControllerTests
    {
        // Badge Response
        [Fact]
        public async Task dispatcher_should_return_badge_equal_to_number_of_badges_on_badge_response()
        {
            var mockedOrdersRepo = new Mock<OrderRepository>().Object;
            var mockedFleetRepoSetup = new Mock<FleetRepository>();
            var mockedDispatchToDroneGatewaySetup = new Mock<DispatchToDroneGateway>();
            
            mockedDispatchToDroneGatewaySetup.Setup(x 
                => x.InitializeRegistration(It.IsAny<InitDroneRequest>()))
                .Returns(Task.FromResult(Constants.TestInitRegistrationResponse)).Verifiable();
            var mockedDispatchToDroneGateway = mockedDispatchToDroneGatewaySetup.Object as DispatchToDroneGateway;
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

            var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            controller.changeGateway(mockedDispatchToDroneGateway);
            var response = await controller.BadgeResponse();
            response.Should().Be(mockedFleetRepo.GetAllAddresses().Result.Count);
        }
        
        // Complete registration
        [Fact]
        public async Task dispatcher_should_return_true_on_complete_registration()
        {
            var mockedOrdersRepo = new Mock<OrderRepository>().Object;
            var mockedFleetRepoSetup = new Mock<FleetRepository>();
            var mockedDispatchToDroneGateway = new DispatchToDroneGateway();
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

            var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            var handlerFactory = new HttpHandlerFactory();
            handlerFactory.OkAllHttp();
            var handle = handlerFactory.GetHttpMessageHandler();
            mockedDispatchToDroneGateway.ChangeHandler(handle);
            mockedDispatchToDroneGateway.Url = Constants.DispatcherIp;
            controller.changeGateway(mockedDispatchToDroneGateway);
            var response = await controller.CompleteRegistration(Constants.TestInitDroneDto);
            response.Should().Be(true);
        }
        
        // patch delivery time
        [Fact]
        public async Task dispatcher_should_return_true_on_patch_delivery_time()
        {
            var mockedOrdersRepoSetup = new Mock<OrderRepository>();
            mockedOrdersRepoSetup.Setup(x => x.PatchTimeCompleted(It.IsAny<string>())).Returns(Task.FromResult(true));
            var mockedOrdersRepo = mockedOrdersRepoSetup.Object;
            var mockedFleetRepoSetup = new Mock<FleetRepository>();
            var mockedDispatchToDroneGateway = new DispatchToDroneGateway();
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

            var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            var handlerFactory = new HttpHandlerFactory();
            handlerFactory.OkAllHttp();
            var handle = handlerFactory.GetHttpMessageHandler();
            mockedDispatchToDroneGateway.ChangeHandler(handle);
            mockedDispatchToDroneGateway.Url = Constants.DispatcherIp;
            controller.changeGateway(mockedDispatchToDroneGateway);
            var response = await controller.PatchDeliveryTime(Constants.TestCompleteOrderRequest);
            response.Should().Be(true);
        }
        
        
        // patch drone status
        [Fact]
        public async Task dispatcher_should_return_ok_on_patch_status()
        {
            var mockedOrdersRepo = new Mock<OrderRepository>().Object;
            var mockedFleetRepoSetup = new Mock<FleetRepository>();
            var mockedDispatchToDroneGatewaySetup = new Mock<DispatchToDroneGateway>();
            mockedDispatchToDroneGatewaySetup.Setup(x => x.InitializeRegistration(
                    It.IsAny<InitDroneRequest>())).Returns(
                Task.FromResult(Constants.TestInitRegistrationResponse)).Verifiable();
            var mockedDispatchToDroneGateway = mockedDispatchToDroneGatewaySetup.Object as DispatchToDroneGateway;
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

            var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            controller.changeGateway(mockedDispatchToDroneGateway);
            var response = await controller.PatchDroneStatus(Constants.TestDroneStatusUpdateRequest);
            response.Should().NotBeNull();
            response.Should().Be("ok");
        }

        // ADDNewDrone
        [Fact]
        public async Task dispatcher_controller_register_should_send_proper_data_to_gateway()
        {
            var mockedOrdersRepo = new Mock<OrderRepository>().Object;
            var mockedFleetRepoSetup = new Mock<FleetRepository>();
            var mockedDispatchToDroneGatewaySetup = new Mock<DispatchToDroneGateway>();
            mockedDispatchToDroneGatewaySetup.Setup(
                x => x.InitializeRegistration(It.IsAny<InitDroneRequest>()))
                .Returns(Task.FromResult(Constants.TestInitRegistrationResponse)).Verifiable();
            var mockedDispatchToDroneGateway = mockedDispatchToDroneGatewaySetup.Object as DispatchToDroneGateway;
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

        var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            controller.changeGateway(mockedDispatchToDroneGateway);
            controller.AddDrone(Constants.BadDroneRequest);
            mockedDispatchToDroneGatewaySetup.VerifyAll();
        }
        

        [Fact]
        public async Task dispatcher_should_return_ok_on_valid_drone_info()
        {
            var mockedOrdersRepo = new Mock<OrderRepository>().Object;
            var mockedFleetRepoSetup = new Mock<FleetRepository>();
            var mockedDispatchToDroneGatewaySetup = new Mock<DispatchToDroneGateway>();
            mockedDispatchToDroneGatewaySetup.Setup(x => x.InitializeRegistration(
                    It.IsAny<InitDroneRequest>())).Returns(Task.FromResult(
                    Constants.TestInitRegistrationResponse)).Verifiable();
            var mockedDispatchToDroneGateway = mockedDispatchToDroneGatewaySetup.Object as DispatchToDroneGateway;
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

            var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            controller.changeGateway(mockedDispatchToDroneGateway);
            var result = controller.AddDrone(Constants.DroneRegistrationInfo);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo("ok");
        }


}
}