using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DomainImplementation.Repositories;
using DroneDispatcher.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Drone.Tests.Controllers.Unit
{
    public class DispatcherControllerTests
    {
        private const string TestDeliveryAddress = "yo mama";
        private const string TestOrderNumber = "123";
        private const string TestCustomerName = "bobby";
        private const string TestOrderId = "some stuff";
        private const string InvalidTestIp = "test_ip";
        private const string ValidTestIp = "172.18.0.0";
        private const string TestDispatcherUrl = "http://" + ValidTestIp + ":4000";
        
        private static readonly Guid TestGuid = new();
        private static readonly DateTimeOffset TestTimeDelivered = DateTimeOffset.UtcNow;
        private static readonly GeoLocation TestDeliveryLocation = new()
        {
            Latitude = 39.74362771992734m, Longitude = -105.00549345883957m
        };
        private static readonly GeoLocation TestHomeLocation = new()
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00561147600774m
        };
        private static readonly Order TestOrder = new()
        {
            DeliveryAddress = TestDeliveryAddress,
            TimeDelivered = TestTimeDelivered,
            Id = TestOrderNumber,
            TimeOrdered = TestTimeDelivered,
            DeliveryLocation = TestDeliveryLocation,
            CustomerName = TestCustomerName
        };

        private static readonly AddOrderDTO AddOrderDto = new()
        {
            DeliveryLocaion = TestDeliveryLocation,
            Id = TestOrderId
        };
        private static readonly DroneRegistrationInfo DroneRegistrationInfo = new() {
            BadgeNumber = TestGuid,
            IpAddress = ValidTestIp
        };
        private static Domain.Entities.Drone TestDrone = new() {
            IpAddress = ValidTestIp,
            Destination = TestDeliveryLocation,
            BadgeNumber = TestGuid,
            CurrentLocation = TestHomeLocation,
            Status = Constants.DroneStatus.READY,
            OrderId = TestOrderId,
            DispatcherUrl = TestDispatcherUrl,
            Id = "TestGuid",
            HomeLocation = TestHomeLocation
        };

        private Mock<IOrdersRepository> _ordersSetup;
        private IOrdersRepository _orders;
        private Mock<IDronesRepository> _dronesSetup;
        private IDronesRepository _drones;
        private Mock<IDroneGateway> _gatewaySetup;
        private IDroneGateway _gateway;
        private IMongoDatabase _database;
        private DispatcherController _controller;

        public DispatcherControllerTests()
        {
            _dronesSetup = new Mock<IDronesRepository>();
            _dronesSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Drone>())).Returns<Domain.Entities.Drone>(x =>Task.FromResult(x));
            _dronesSetup.Setup(x => x.GetByIdAsync(It.IsAny<string>()));
            _dronesSetup.Setup(x => x.GetAllAvailableDronesAsync()).ReturnsAsync(new List<Domain.Entities.Drone>{TestDrone}.AsEnumerable());
            _drones = _dronesSetup.Object;
            
            _gatewaySetup = new Mock<IDroneGateway>();
            _gatewaySetup.Setup(x => x.StartRegistration(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<GeoLocation>())).ReturnsAsync(true).Verifiable();
            _gatewaySetup.Setup(x => x.AssignDelivery( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>())).ReturnsAsync(true).Verifiable();
            _gateway = _gatewaySetup.Object;
            
            _database = new Mock<IMongoDatabase>().Object;
            
            _ordersSetup = new Mock<IOrdersRepository>();
            _ordersSetup.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(TestOrder).Verifiable();
            _ordersSetup.Setup(x => x.Update(TestOrder)).ReturnsAsync(TestOrder).Verifiable();
            _orders = _ordersSetup.Object;
            _controller = new DispatcherController(_drones, _orders, _gateway);
        }
        
        [Fact]
        public async Task DispatcherControllerShouldReturnOk()
        {
            var result = await _controller.UpdateStatus(new UpdateStatusDto());
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }
        
        // RegisterNewDrone
        [Fact]
        public async Task DispatcherControllerRegisterShouldSendProperDataToGateway()
        {
            _gatewaySetup = new Mock<IDroneGateway>();
            _gateway = _gatewaySetup.Object;
            await _controller.RegisterNewDrone(DroneRegistrationInfo);
            _gatewaySetup.Verify();
        }

        [Fact]// InvalidRegistrationNotOk
        public async Task DispatcherControllerShouldReturnProblemOnIncorrectDroneInfo()
        {
            var badDroneInfo = new DroneRegistrationInfo
            {
                BadgeNumber = TestGuid,
                IpAddress = InvalidTestIp
            };
            var result = await _controller.RegisterNewDrone(badDroneInfo);
            // Problem object result used in register drone that is invalid for now, will change with black box changes.
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]// ValidRegistrationOk
        public async Task DispatcherShouldReturnOkOnValidDroneInfo()
        {
            var result = await _controller.RegisterNewDrone(DroneRegistrationInfo);
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }

        
        // OrderAssignedIfDroneAvailable
        [Fact]
        public async Task AddNewOrderShouldAssignAnOrderIfAvailable()
        {
            // calling method
            _ordersSetup = new Mock<IOrdersRepository>();
            _dronesSetup = new Mock<IDronesRepository>();
            _gatewaySetup = new Mock<IDroneGateway>();
            _dronesSetup.Setup(x => x.GetAllAvailableDronesAsync()).ReturnsAsync(new List<Domain.Entities.Drone>{TestDrone});
            // _dronesSetup.Setup(x => x.GetByIdAsync(It.IsAny<string>()));
            _ordersSetup.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(TestOrder).Verifiable();
            _gatewaySetup.Setup(x => x.AssignDelivery( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>())).ReturnsAsync(true).Verifiable();
            _controller = new DispatcherController(_drones, _orders, _gateway);
            await _controller.AddNewOrder(AddOrderDto);
            _gatewaySetup.Verify(x => x.AssignDelivery( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>()));
        }
        
        [Fact]// AddNewOrderOk
        public async Task AddNewOrderShouldReturnOk()
        {
            var result = await _controller.AddNewOrder(AddOrderDto);
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
        
        // CompleteDeliveryOk
        [Fact]
        public async Task CompleteDeliveryShouldReturnOk()
        {
            // calling method
            var result = await _controller.CompleteDelivery(TestOrderNumber);
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
            _ordersSetup.Verify();
        }



        private static IDronesRepository OrderSetup(out IDroneGateway mockedDroneGateway,
            out Mock<IOrdersRepository> mockedOrdersRepositorySetup, out string testOrderNumber)
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            mockedDroneGateway = new Mock<IDroneGateway>().Object;
            mockedOrdersRepositorySetup = new Mock<IOrdersRepository>();
            testOrderNumber = "35";
            var testOrder = new Order
            {
                Id = testOrderNumber,
                CustomerName = "testee",
                DeliveryLocation = new GeoLocation
                {
                    Latitude = 69,
                    Longitude = 69
                },
                TimeOrdered = DateTimeOffset.UtcNow
                // TimeDelivered assumed null by business logic
            };

            // Ensuring orders repo is updated
            var s = testOrderNumber;
            mockedOrdersRepositorySetup.Setup(x => x.GetByIdAsync(s)).Returns(Task.FromResult(testOrder))
                .Verifiable();
            mockedOrdersRepositorySetup.Setup(x => x.Update(testOrder)).Returns(Task.FromResult(testOrder)).Verifiable();
            return mockedDronesRepository;
        }

        [Fact] // CompleteDeliveryUpdatesOrders
        public async Task CompleteDeliveryShouldUpdateOrders()
        {
            await _controller.CompleteDelivery(TestOrderId);
            _ordersSetup.Verify();
        }

        [Fact]
        public async Task DroneIsReadyForOrdersShouldAlwaysReturnOk()
        {
            var result = await _controller.DroneIsReadyForOrder($"{TestGuid}");
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact] //ReadyDroneGetsCorrectOrder
        public async Task DroneIsReadyForOrdersShouldAssignTheCorrectOrder()
        {
            await _controller.AddNewOrder(AddOrderDto);
            // calling method
            await _controller.DroneIsReadyForOrder($"{TestGuid}");
        
            // Checking assign has right parameters
            _gatewaySetup.Verify();
        }
        
        [Fact] // 
        public async Task DroneIsReadyForOrdersShouldGetAsyncCorrectly()
        {
            await _controller.AddNewOrder(AddOrderDto);
            // calling method
            await _controller.DroneIsReadyForOrder($"{TestGuid}");
            // Checking getasync has right parameters
            _gatewaySetup.Verify();
        }
    }
}