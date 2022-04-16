// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Net.Http;
// using System.Threading;
// using System.Threading.Tasks;
// using Dispatch.Controllers;
// using Domain.DTO;
// using Domain.DTO.DroneDispatchCommunication;
// using Domain.Entities;
// using Domain.InterfaceDefinitions.Gateways;
// using Domain.InterfaceDefinitions.Repositories;
// using Domain.InterfaceImplementations.Gateways;
// using FluentAssertions;
// using FrontEnd.Services;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Moq.Protected;
// using SimDrone;
//
// using SimDrone.Controllers;
// using Xunit;
// using System.Linq;
// using Domain.InterfaceImplementations.Repositories;
// using Microsoft.Extensions.Options;
// using MongoDB.Driver;
//
// namespace Tests.Controllers.EndToEnd
// {
//     public class DispatcherConnectionsTests
//     {
//         // private void checkStuff()
//         // {
//         //     var simDroneSetup = new Mock<SimDroneController>();
//         //     simDroneSetup.Setup(x 
//         //         => x.InitializeRegistration(
//         //             It.IsAny<InitDroneRequest>())).Returns<Task<string?>>();
//         // private Dictionary<string, IActionResult> methodMap =
//         //     new Dictionary<string, IActionResult>(
//         //             "SimDrone.InitializeRegistration"  
//         //         );
//         // }
//
//         /*private static void SetUpAll(out DispatchController controller, out Mock<IOrdersRepository> orders, out Mock<IDroneGateway> gateway, params  MethodSetups[] toggles)
//         {
//             var drones = new Mock<IDronesRepository>();
//             orders = new Mock<IOrdersRepository>();
//             gateway = new Mock<IDroneGateway>();
//             if(toggles.Contains(MethodSetups.DroneShouldCreateAsync)) 
//                 drones.Setup(x => x.CreateAsync(It.IsAny<Drone>())).Returns<Drone>(Task.FromResult);
//             if(toggles.Contains(MethodSetups.DroneShouldGetByIdAsync)) 
//                 drones.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(Drone()));
//             if(toggles.Contains(MethodSetups.DroneShouldGetAllAvailableDronesAsync))
//                 drones.Setup(x => x.GetAllAvailableDronesAsync()).ReturnsAsync(new List<Drone>{TestDroneRecord}.AsEnumerable());
//             if(toggles.Contains(MethodSetups.OrderShouldGetByIdAsync))
//                 orders.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(TestOrder).Verifiable();
//             if(toggles.Contains(MethodSetups.OrderShouldUpdate))
//                 orders.Setup(x => x.Update(TestOrder)).ReturnsAsync(TestOrder).Verifiable();
//             if(toggles.Contains(MethodSetups.GatewayShouldStartRegistration))
//                 gateway.Setup(x => x.StartRegistration(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<GeoLocation>())).ReturnsAsync(true).Verifiable();
//             if(toggles.Contains(MethodSetups.GatewayShouldAssignDelivery2))
//                 gateway.Setup(x => x.AssignDelivery(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<GeoLocation>())).Verifiable();
//             controller = new DispatcherController(drones.Object, orders.Object, gateway.Object);
//         }*/
//         //completeRegistration
//         [Fact]
//         public async Task dispatcher_controller_should_finish_registration_to_drone()
//         {
//
//             // Mocked items
//             var arrayOfDrones = new DroneRecord[] {Constants.TestRecord, Constants.TestRecord};
//             var arrayOfOrders = new Order[] {Constants.TestOrder, Constants.TestOrder};
//
//             var databaseFactory = new ArrayMockDatabaseFactory(arrayOfDrones, arrayOfOrders);
//             var mockDroneMongo = databaseFactory.getDroneDatabase();
//             var mockOrderMongo = databaseFactory.getOrderDatabase();
//             
//             var mockFleetRepo = new FleetRepository(mockDroneMongo);
//             var mockOrdersRepo = new OrderRepository(mockOrderMongo);
//
//
//             // Portions of whole system
//             //var frontEndGateway = new FrontEndToDispatchGateway();
//             // TODO: unsupported no adding drones yet from frontend
//             var testDroneToDispatchGateway = new DroneToDispatchGateway();
//             var testDispatchToDroneGateway = new DispatchToDroneGateway();
//             var testDroneController = new SimDroneController();
//             var testDispatcher = new DispatchController(mockFleetRepo, mockOrdersRepo);
//             var testDroneSim = new Drone(Constants.TestRecord, testDroneToDispatchGateway);
//
//             // Setting up offline sockets
//             var httpFactory = new HttpHandlerFactory();
//             httpFactory.SetupHttpMethod("http://test_ip/drone/StartDrone", testDroneController.StartDrone,
//                 testDroneSim);
//
//
//             // Calls
//             var response = await testDispatchToDroneGateway.CompleteRegistration(Constants.TestRecord);
//
//             // Comparisons
//             var expected = new OkResult();
//             response.Should().BeEquivalentTo(expected);
//         }
//
//
//         //initRegistration
//         // [Fact]
//         // public async Task dispatcher_controller_should_start_registration_to_drone()
//         // {
//         //     var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
//         //     var testDroneInfo = new AssignFleetRequest
//         //     {
//         //         BadgeNumber = 5,
//         //         DispatcherUrl = Constants.Url,
//         //         HomeLocation = Constants.HomeLocation
//         //     };
//         //     var frontToDispatch = new Mock<FrontEndToDispatchGateway>().Object;
//         //     
//         //     
//         //     
//         //     var mockedDronesRepo = new Mock<IDronesRepository>().Object;
//         //     var mockedDispatcherGateway = new Mock<DroneToDispatchGateway>().Object;
//         //     var testDroneGateway = new DispatchToDroneGateway();
//         //     var testDroneController = new SimDroneController(new DroneToDispatchGateway());
//         //     var testDispatcherController = new DispatcherController(mockedDronesRepo,mockedOrdersRepo,testDroneGateway);
//         //     var mockedDroneHandlerSetup = new Mock<HttpMessageHandler>();
//         //     var mockedDispatcherHandlerSetup = new Mock<HttpMessageHandler>();
//         //     mockedDroneHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"http://test_ip/drone/initregistration")), ItExpr.IsAny<CancellationToken>())
//         //         .ReturnsAsync(new HttpResponseMessage {
//         //             StatusCode = HttpStatusCode.OK,
//         //             Content = new StringContent(testDroneController.InitializeRegistration().IsCompletedSuccessfully.ToString()
//         //             )
//         //             // Assumed ok only for now
//         //         });
//         //     mockedDroneHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"http://test_ip/drone/completeregistration")), ItExpr.IsAny<CancellationToken>())
//         //         .ReturnsAsync(new HttpResponseMessage {
//         //             StatusCode = HttpStatusCode.OK,
//         //             Content = new StringContent(testDroneController.CompleteRegistration().IsCompletedSuccessfully.ToString()
//         //             )
//         //             // Assumed ok only for now
//         //         });
//         //     var mockedDroneHandler = mockedDroneHandlerSetup.Object;
//         //     mockedDispatcherHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"http://test_ip/drone/register")), ItExpr.IsAny<CancellationToken>())
//         //         .ReturnsAsync(new HttpResponseMessage {
//         //             StatusCode = HttpStatusCode.OK,
//         //             Content = new StringContent(testDroneGateway.AssignToFleet(testDroneInfo.DispatchIp).IsCompletedSuccessfully.ToString()
//         //             )
//         //             // Assumed ok only for now
//         //         });
//         //     var mockedDispatcherHandler = mockedDispatcherHandlerSetup.Object;
//         //     DispatchToDroneGateway.ChangeHandler(mockedDroneHandler);
//         //     var testDispatcherGateway = new DroneToDispatchGateway();
//         //     testDispatcherGateway.ChangeHandler(mockedDispatcherHandler);
//         //     // Mocking http server
//         //     var response = await testDispatcherController.RegisterNewDrone(testDroneInfo);
//         //     var expected = new OkResult();
//         //     response.Should().BeEquivalentTo(expected);
//         // }
//         //
//         //     // AssignDelivery
//         // [Fact]
//         // public async Task dispatcher_controller_should_assign_delivery_to_drone()
//         // {
//         //     var testDroneController = new DroneController(mockedDroneRepo, droneToDispatchGateway);
//         //     var dispatchToDroneGateway = new DispatchToDroneGateway();
//         //     
//         //     // This is accidentally end-to-end
//         //     var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
//         //     var mockedDroneRepo = new Mock<IDronesRepository>().Object;
//         //     var droneToDispatchGateway = new Mock<DroneToDispatchGateway>().Object;
//         //
//         //     var testDeliverOrderDto = new AssignDeliveryRequest
//         //     {
//         //        OrderId = "testOrderId",
//         //        OrderLocation = new GeoLocation
//         //        {
//         //            Latitude = 39.7440m,
//         //            Longitude = -105.0010m
//         //        }
//         //     };
//         //
//         //     var testDispatcherController = new Mock<DispatchController>().Object;
//         //     var mockedDroneHandlerSetup = new Mock<HttpMessageHandler>();
//         //     var mockedHandlerSetup = new Mock<HttpMessageHandler>();
//         //     mockedHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//         //         .ReturnsAsync(new HttpResponseMessage {
//         //             StatusCode = HttpStatusCode.OK,
//         //             Content = new StringContent("we skip these calls")
//         //             // Assumed ok only for now
//         //         });
//         //     mockedDroneHandlerSetup.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"http://172.18.0.1:5001/drone/assigndelivery")), ItExpr.IsAny<CancellationToken>())
//         //         .ReturnsAsync(new HttpResponseMessage {
//         //             StatusCode = HttpStatusCode.OK,
//         //             Content = new StringContent(testDroneController.Deliver(testDeliverOrderDto).IsCompletedSuccessfully.ToString())
//         //             // Assumed ok only for now
//         //         });
//         //     var mockedHandler = mockedHandlerSetup.Object;
//         //     DispatchToDroneGateway.ChangeHandler(mockedHandler);
//         //     var testDrone = new Drone( Constants.TestRecord, droneToDispatchGateway);
//         //     
//         //     testDroneController.changeDrone(testDrone);
//         //     var frontToDispatcher = new Mock<FrontEndToDispatchGateway>();
//         //     frontToDispatcher.
//         //     toDispatcher.ChangeHandler(mockedHandler);
//         //     // Mocking http server
//         //     var response = await testDispatcherController.AddNewOrder(testOrderDto);
//         //     var expected = new OkResult();
//         //     response.Should().BeEquivalentTo(expected);
//         //     testDrone.CurrentLocation.Should().BeEquivalentTo(testDeliverOrderDto.OrderLocation);
//         //
//         // }
//     }
// }