using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
using Domain.DTO.DispatcherFrontEnd.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Implementation.Gateways;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DroneDispatcher.Controllers;
using DroneSimulator.Controllers;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;
using Xunit;

namespace Drone.Tests.Controllers.EndToEnd
{
    public class DispatcherConnectionsTests
    {
        
            //completeRegistration
            [Fact]
        public async Task dispatcher_controller_should_finish_registration_to_drone()
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            //testDroneGateway.changeHandler(mockedDroneHandler);
            var testDispatcherGateway = new DispatcherGateway();
            //testDispatcherGateway.changeHandler(mockedDispatcherHandler);
            // Mocking http server
            var testDroneController = new DroneController(mockedDronesRepository, testDispatcherGateway);
            var response = await testDroneController.CompleteRegistration();

            var expected = new OkResult();
            response.Should().BeEquivalentTo(expected);
        }
            
            
            //initRegistration
        [Fact]
        public async Task dispatcher_controller_should_start_registration_to_drone()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            var testDroneInfo = new DroneRegistrationInfo
            {
                BadgeNumber = new Guid(),
                IpAddress = "test_ip"
            };
            var mockedDronesRepo = new Mock<IDronesRepository>().Object;
            var mockedDispatcherGateway = new Mock<IDispatcherGateway>().Object;
            var testDroneGateway = new DroneGateway();
            var testDroneController = new DroneController(mockedDronesRepo, mockedDispatcherGateway);
            var testDispatcherController = new DispatcherController(mockedDronesRepo,mockedOrdersRepo,testDroneGateway);
            var mockedDroneHandlerSetup = new Mock<HttpMessageHandler>();
            var mockedDispatcherHandlerSetup = new Mock<HttpMessageHandler>();
            mockedDroneHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"http://test_ip/drone/initregistration")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testDroneController.InitializeRegistration().IsCompletedSuccessfully.ToString()
                    )
                    // Assumed ok only for now
                });
            mockedDroneHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"http://test_ip/drone/completeregistration")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testDroneController.CompleteRegistration().IsCompletedSuccessfully.ToString()
                    )
                    // Assumed ok only for now
                });
            var mockedDroneHandler = mockedDroneHandlerSetup.Object;
            mockedDispatcherHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"http://test_ip/drone/register")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testDroneGateway.OKToSendStatus(testDroneInfo.IpAddress).IsCompletedSuccessfully.ToString()
                    )
                    // Assumed ok only for now
                });
            var mockedDispatcherHandler = mockedDispatcherHandlerSetup.Object;
            testDroneGateway.changeHandler(mockedDroneHandler);
            var testDispatcherGateway = new DispatcherGateway();
            testDispatcherGateway.changeHandler(mockedDispatcherHandler);
            // Mocking http server
            var response = await testDispatcherController.RegisterNewDrone(testDroneInfo);
            var expected = new OkResult();
            response.Should().BeEquivalentTo(expected);
        }
        
            // AssignDelivery
        [Fact]
        public async Task dispatcher_controller_should_assign_delivery_to_drone()
        {
            // This is accidentally end-to-end
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            var mockedDroneRepo = new Mock<IDronesRepository>().Object;
            var mockedDispatcherGateway = new Mock<IDispatcherGateway>().Object;
            var testDroneController = new DroneController(mockedDroneRepo, mockedDispatcherGateway);
            var testDroneGateway = new DroneGateway();

            var testDeliverOrderDto = new DeliverOrderDto
            {
               OrderId = "testOrderId",
               OrderLocation = new GeoLocation
               {
                   Latitude = 39.7440m,
                   Longitude = -105.0010m
               }
            };
            var testOrderDto = new PostAddOrderDto
            {
                DeliveryLocaion = new GeoLocation
                {
                    Latitude = 39.74273568191456m,
                    Longitude = -105.00771026053671m
                },
                Id = "testOrderID"
            };
         
            var testDispatcherController = new DispatcherController(mockedDroneRepo,mockedOrdersRepo,testDroneGateway);
            var mockedDroneHandlerSetup = new Mock<HttpMessageHandler>();
            var mockedHandlerSetup = new Mock<HttpMessageHandler>();
            mockedHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("we skip these calls")
                    // Assumed ok only for now
                });
            mockedDroneHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"http://172.18.0.1:5001/drone/assigndelivery")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testDroneController.AssignDelivery(testDeliverOrderDto).IsCompletedSuccessfully.ToString())
                    // Assumed ok only for now
                });
            var mockedHandler = mockedHandlerSetup.Object;
            testDroneGateway.changeHandler(mockedHandler);
            var testDrone = new DroneSimulator.Drone("test_badge", testDeliverOrderDto.OrderLocation, mockedDispatcherGateway);
            testDroneController.changeDrone(testDrone);
            var testDispatcherGateway = new DispatcherGateway();
            testDispatcherGateway.changeHandler(mockedHandler);
            // Mocking http server
            var response = await testDispatcherController.AddNewOrder(testOrderDto);
            var expected = new OkResult();
            response.Should().BeEquivalentTo(expected);
            testDrone.Location.Should().BeEquivalentTo(testDeliverOrderDto.OrderLocation);
        }
    }
}