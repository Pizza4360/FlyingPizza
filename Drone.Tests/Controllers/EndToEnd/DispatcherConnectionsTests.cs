using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
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
        
            //initRegistration
        
        
            // AssignDelivery
        [Fact]
        public async Task dispatcher_controller_should_assign_delivery_to_drone()
        {
            // This is accidentally end-to-end
            var testDeliverOrderDto = new DeliverOrderDto
            {
               OrderId = "testOrderId",
               OrderLocation = new GeoLocation
               {
                   Latitude = 39.74364421910773m,
                   Longitude = -105.00561147600774m
               }
            };
            var testOrderDto = new AddOrderDTO
            {
                DeliveryLocaion = new GeoLocation
                {
                    Latitude = 39.74364421910773m,
                    Longitude = -105.00561147600774m
                },
                Id = "testOrderID"
            };
            var mockedDronesRepo = new Mock<IDronesRepository>().Object;
            var mockedDispatcherGateway = new Mock<IDispatcherGateway>().Object;
            var testDroneGateway = new DroneGateway();
            var testDroneController = new DroneController(mockedDronesRepo, mockedDispatcherGateway);
            var testDispatcherController = new DispatcherController(mockedDronesRepo,testDroneGateway);
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
                    Content = new StringContent(testDroneController.AssignDelivery(testDeliverOrderDto).IsCompletedSuccessfully.ToString()
                    )
                    // Assumed ok only for now
                });
            var mockedHandler = mockedHandlerSetup.Object;
            testDroneGateway.changeHandler(mockedHandler);
            var testDispatcherGateway = new DispatcherGateway();
            testDispatcherGateway.changeHandler(mockedHandler);
            // Mocking http server
            var response = await testDispatcherController.AddNewOrder(testOrderDto);
            var expected = new OkResult();
            response.Should().BeEquivalentTo(expected);
        }

    }
}