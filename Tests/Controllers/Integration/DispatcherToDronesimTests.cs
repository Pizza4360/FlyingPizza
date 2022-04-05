using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dispatcher.Controllers;
using Domain.DTO.DroneToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DomainImplementation.Gateways;
using DroneSimulator.Controllers;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace Drone.Tests.Controllers.Integration
{
    public class DispatcherToDronesim
    {
        
        // Dispatcher gateway - DispatcherController
        [Fact]
        public async Task dispatcher_gateway_should_update_status_with_dispatcher()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            var testUpdateDto = new PutStatusDto
            {
                BadgeNumber = "something",
                Location = new GeoLocation
                {
                    Latitude = 69,
                    Longitude = 69
                },
                State = DroneState.Ready
            };
            var mockedDronesRepo = new Mock<IDronesRepository>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var testDispatcherController = new DispatcherController(mockedDronesRepo,mockedOrdersRepo,mockedDroneGateway);
            var mockedHandlerSetup = new Mock<HttpMessageHandler>();
                mockedHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testDispatcherController.UpdateStatus(testUpdateDto).IsCompletedSuccessfully.ToString())
                    // Assumed ok only for now
                });
                var mockedHandler = mockedHandlerSetup.Object;
    
            var testDispatcherGateway = new DroneToDispatcherGateway();
            testDispatcherGateway.changeHandler(mockedHandler);
            // Mocking http server

          
            
            var response = await testDispatcherGateway.PutDroneState(testUpdateDto);
            response.Should().BeTrue();
        }
        
        // DroneGateway - DroneController
        [Fact]
        public async Task drone_gateway_should_register_with_drone_sim()
        {
            var mockedDronesRepo = new Mock<IDronesRepository>().Object;
            var mockedDispatcherGateway = new Mock<IDispatcherGateway>().Object;
            var testDroneGateway = new DroneGateway();
            var mockedHandlerSetup = new Mock<HttpMessageHandler>();
            var testDroneController = new DroneController(mockedDronesRepo, mockedDispatcherGateway);
            mockedHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testDroneController.InitializeRegistration().IsCompletedSuccessfully.ToString())
                    // Assumed ok only for now
                });
            var mockedHandler = mockedHandlerSetup.Object;
            testDroneGateway.changeHandler(mockedHandler);
            // Mocking http server

          
            
            var response = await testDroneGateway.StartRegistration("test_ip", new Guid(), "http://172.18.0.0:4000/dispatcher",new GeoLocation{ Latitude = 69, Longitude = 69});
            response.Should().BeTrue();
        }
    }
}