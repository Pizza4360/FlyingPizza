using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.DTO.DispatcherDrone.DispatcherToDrone;
using Domain.DTO.DispatcherDrone.DroneToDispatcher;
using Domain.DTO.DispatcherFrontEnd.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DroneDispatcher.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Drone.Tests.Controllers.Unit
{
    public class DispatcherControllerTests
    {
        private const string
            TestDeliveryAddress = "yo mama",
            TestOrderNumber = "123",
            TestCustomerName = "bobby",
            TestOrderId = "some stuff",
            InvalidTestIp = "test_ip",
            ValidTestIp = "172.18.0.0",
            TestDispatcherUrl = "http://" + ValidTestIp + ":4000";
            enum MethodSetups
            {
                DroneShouldCreateAsync,
                DroneShouldGetByIdAsync,
                DroneShouldGetAllAvailableDronesAsync,
                OrderShouldGetByIdAsync,
                OrderShouldUpdate,
                GatewayShouldStartRegistration,
                GatewayShouldAssignDelivery2,
            }
        private static readonly GeoLocation 
            TestDeliveryLocation = new() 
            {
                Latitude = 39.74362771992734m, Longitude = -105.00549345883957m
            },
            TestHomeLocation = new()
            {
                Latitude = 39.74364421910773m,
                Longitude = -105.00561147600774m
            };
        
        private static readonly Guid TestGuid = new();
        private static readonly DateTimeOffset TestTimeDelivered = DateTimeOffset.UtcNow;
        
        private static readonly Order TestOrder = new()
        {
            DeliveryAddress = TestDeliveryAddress,
            TimeDelivered = TestTimeDelivered,
            Id = TestOrderNumber,
            TimeOrdered = TestTimeDelivered,
            DeliveryLocation = TestDeliveryLocation,
            CustomerName = TestCustomerName
        };
        private static readonly PostDroneRegistrationDto 
            BadPostDroneDto = new()
            {
                BadgeNumber = TestGuid,
                IpAddress = InvalidTestIp
            }, 
            PostDroneRegistrationDto = new() {
                BadgeNumber = TestGuid,
                IpAddress = ValidTestIp
            };

        private static readonly PostAddOrderDto AddOrderDto = new()
        {
            DeliveryLocaion = TestDeliveryLocation,
            Id = TestOrderId
        };
        private static readonly Domain.Entities.DroneRecord TestDrone = new() {
            IpAddress = ValidTestIp,
            Destination = TestDeliveryLocation,
            BadgeNumber = TestGuid,
            CurrentLocation = TestHomeLocation,
            State = DroneState.Ready,
            OrderId = TestOrderId,
            DispatcherUrl = TestDispatcherUrl,
            Id = "TestGuid",
            HomeLocation = TestHomeLocation
        };

        private static void SetUpAll(out DispatcherController controller, out Mock<IOrdersRepository> orders, out Mock<IDroneGateway> gateway, params  MethodSetups[] toggles)
        {
            var drones = new Mock<IDronesRepository>();
            orders = new Mock<IOrdersRepository>();
            gateway = new Mock<IDroneGateway>();
            if(toggles.Contains(MethodSetups.DroneShouldCreateAsync))
                drones.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns<Domain.Entities.DroneRecord>(Task.FromResult);
            if(toggles.Contains(MethodSetups.DroneShouldGetByIdAsync))
                drones.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Domain.Entities.DroneRecord()));
            if(toggles.Contains(MethodSetups.DroneShouldGetAllAvailableDronesAsync))
                drones.Setup(x => x.GetAllAvailableDronesAsync()).ReturnsAsync(new List<Domain.Entities.DroneRecord>{TestDrone}.AsEnumerable());
            if(toggles.Contains(MethodSetups.OrderShouldGetByIdAsync))
                orders.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(TestOrder).Verifiable();
            if(toggles.Contains(MethodSetups.OrderShouldUpdate))
                orders.Setup(x => x.Update(TestOrder)).ReturnsAsync(TestOrder).Verifiable();
            if(toggles.Contains(MethodSetups.GatewayShouldStartRegistration))
                gateway.Setup(x => x.StartRegistration(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<GeoLocation>())).ReturnsAsync(true).Verifiable();
            if(toggles.Contains(MethodSetups.GatewayShouldAssignDelivery2))
                gateway.Setup(x => x.AssignDelivery(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<GeoLocation>())).Verifiable();
            controller = new DispatcherController(drones.Object, orders.Object, gateway.Object);
        }

        [Fact]
        public async Task DispatcherControllerShouldReturnOk()
        {
            SetUpAll(out var controller, out _, out _);
            var result = await controller.UpdateStatus(new PutStatusDto());
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }
        
        // RegisterNewDrone
        [Fact]
        public async Task DispatcherControllerRegisterShouldSendProperDataToGateway()
        {
            SetUpAll(out var controller, out _, out var gateway, MethodSetups.GatewayShouldStartRegistration, MethodSetups.DroneShouldCreateAsync);
            await controller.RegisterNewDrone(PostDroneRegistrationDto);
            gateway.Verify();
        }

        [Fact]// InvalidRegistrationNotOk
        public async Task DispatcherControllerShouldReturnProblemOnIncorrectDroneInfo()
        {
            SetUpAll(out var controller, out _, out _);
            var result = await controller.RegisterNewDrone(BadPostDroneDto);
            // Problem object result used in register drone that is invalid for now, will change with black box changes.
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

         [Fact]// ValidRegistrationOk
        public async Task DispatcherShouldReturnOkOnValidDroneInfo()
        {
            SetUpAll(out var controller, out _, out _, MethodSetups.DroneShouldCreateAsync, MethodSetups.GatewayShouldStartRegistration);
            var result = await controller.RegisterNewDrone(PostDroneRegistrationDto);
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }

        
        // OrderAssignedIfDroneAvailable
        [Fact]
        public async Task AddNewOrderShouldAssignAnOrderIfAvailable()
        {
            SetUpAll(out var controller, out _, out var gateway,
                MethodSetups.DroneShouldGetAllAvailableDronesAsync, 
                MethodSetups.DroneShouldGetByIdAsync,
                MethodSetups.OrderShouldGetByIdAsync, 
                MethodSetups.GatewayShouldAssignDelivery2
            );
            await controller.AddNewOrder(AddOrderDto);
            gateway.Verify(x => x.AssignDelivery( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>()));
        }
        
        [Fact]// AddNewOrderOk
        public async Task AddNewOrderShouldReturnOk()
        {
            SetUpAll(out var controller, out _, out _, 
                MethodSetups.GatewayShouldAssignDelivery2);
            var result = await controller.AddNewOrder(AddOrderDto);
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
        
        // CompleteDeliveryOk
        [Fact]
        public async Task CompleteDeliveryShouldReturnOk()
        {
            SetUpAll(out var controller, out var orders, out _, 
                MethodSetups.OrderShouldGetByIdAsync,
                MethodSetups.OrderShouldUpdate,
                MethodSetups.GatewayShouldAssignDelivery2);
            // calling method
            var result = await controller.CompleteDelivery(TestOrderNumber);
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
            orders.Verify();
        }
        

        [Fact] // CompleteDeliveryUpdatesOrders
        public async Task CompleteDeliveryShouldUpdateOrders()
        {
            SetUpAll(out var controller, out var orders, out _,
                MethodSetups.OrderShouldGetByIdAsync,
                MethodSetups.OrderShouldUpdate);
            await controller.CompleteDelivery(TestOrderId);
            orders.Verify();
        }

        [Fact]
        public async Task DroneIsReadyForOrdersShouldAlwaysReturnOk()
        {
            SetUpAll(out var controller, out _, out _);
            var result = await controller.DroneIsReadyForOrder($"{TestGuid}");
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact] //ReadyDroneGetsCorrectOrder
        public async Task DroneIsReadyForOrdersShouldAssignTheCorrectOrder()
        {
            SetUpAll(out var controller, out _, out var gateway,
                MethodSetups.OrderShouldGetByIdAsync, 
                MethodSetups.GatewayShouldAssignDelivery2,
                MethodSetups.DroneShouldGetByIdAsync);
            await controller.AddNewOrder(AddOrderDto);
            // calling method
            await controller.DroneIsReadyForOrder($"{TestGuid}");
        
            // Checking assign has right parameters
            gateway.VerifyAll();
        }
        
        [Fact] // 
        public async Task DroneIsReadyForOrdersShouldGetAsyncCorrectly()
        {
            SetUpAll(out var controller, out _, out var gateway,
                MethodSetups.OrderShouldGetByIdAsync, 
                MethodSetups.DroneShouldGetByIdAsync,
                MethodSetups.DroneShouldGetByIdAsync);
            await controller.AddNewOrder(AddOrderDto);
            // calling method
            await controller.DroneIsReadyForOrder($"{TestGuid}");
            // Checking get async has right parameters
            gateway.Verify();
        }
    }
}