using System.Collections.Generic;
using System.Threading.Tasks;
using Dispatch.Controllers;
using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceDefinitions.Repositories;
using Domain.InterfaceImplementations.Gateways;
using Domain.InterfaceImplementations.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Tests.Controllers.Unit
{


    public class DispatcherControllerTests
    {
        // Badge Response
        // Complete registration
        // patch delivery time
        // patch drone status
        

        // ADDNewDrone
        [Fact]
        public async Task dispatcher_controller_register_should_send_proper_data_to_gateway()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object as OrderRepository;
            var mockedFleetRepoSetup = new Mock<IFleetRepository>();
            var mockedDispatchToDroneGatewaySetup = new Mock<IDispatchToDroneGateway>();
            mockedDispatchToDroneGatewaySetup.Setup(x => x.InitializeRegistration(Constants.DroneIp, Constants.Url, Constants.TestBadgeNumber)).Returns(Task.FromResult(true)).Verifiable();
            var mockedDispatchToDroneGateway = mockedDispatchToDroneGatewaySetup.Object as DispatchToDroneGateway;
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

        var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            controller.changeGateway(mockedDispatchToDroneGateway);
            await controller.AddDrone(Constants.TestGatewayDto);
            mockedDispatchToDroneGatewaySetup.VerifyAll();
        }

        [Fact]
        public async Task dispatcher_controller_should_return_problem_on_incorrect_drone_info()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object as OrderRepository;
            var mockedFleetRepoSetup = new Mock<IFleetRepository>();
            var mockedDispatchToDroneGatewaySetup = new Mock<IDispatchToDroneGateway>();
            mockedDispatchToDroneGatewaySetup.Setup(x => x.InitializeRegistration("", "", -5)).Returns(Task.FromResult(false));
            var mockedDispatchToDroneGateway = mockedDispatchToDroneGatewaySetup.Object as DispatchToDroneGateway;
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

            var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            var result = await controller.AddDrone(Constants.TestGatewayDto);
            result.Should().NotBeNull();
            result.Should().NotBe("ok");
        }

        [Fact]
        public async Task dispatcher_should_return_ok_on_valid_drone_info()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object as OrderRepository;
            var mockedFleetRepoSetup = new Mock<IFleetRepository>();
            var mockedDispatchToDroneGatewaySetup = new Mock<IDispatchToDroneGateway>();
            mockedDispatchToDroneGatewaySetup.Setup(x => x.InitializeRegistration(Constants.DroneIp, Constants.Url, Constants.TestBadgeNumber)).Returns(Task.FromResult(true)).Verifiable();
            var mockedDispatchToDroneGateway = mockedDispatchToDroneGatewaySetup.Object as DispatchToDroneGateway;
            mockedFleetRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns(Task.FromResult(true));
            mockedFleetRepoSetup.Setup(x => x.GetAllAddresses())
                .Returns(Task.FromResult(Constants.TestStringDict));

            var mockedFleetRepo = mockedFleetRepoSetup.Object;
            var controller =
                new DispatchController(mockedFleetRepo, mockedOrdersRepo);
            controller.changeGateway(mockedDispatchToDroneGateway);
            var result = await controller.AddDrone(Constants.TestGatewayDto);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo("ok");
        }


}
}