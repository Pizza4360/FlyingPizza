using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dispatch.Controllers;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceDefinitions.Repositories;
using Domain.InterfaceImplementations.Gateways;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using SimDrone;
using SimDrone.Controllers;
using Xunit;

namespace Tests.Controllers.Unit
{
    public class DispatcherControllerTests
    {
        [Fact]
        public async Task DroneSimDeliverShouldReturnTrue()
        {
            // Assumed to return an ok object result with ok as arg

            var mockedGateway = new Mock<IDroneToDispatcherGateway>();
            mockedGateway.Setup(x => x.PatchDroneStatus(It.IsAny<DroneStatusUpdateRequest>()))
                .Returns(Task.FromResult<BaseDto>(Constants.TestDroneStatusUpdateResponse));
            var adapter = new Drone(Constants.TestRecord, mockedGateway.Object);
            var response = adapter.DeliverOrder(Constants.TestOrder.DeliveryLocation);
            response.Should().BeTrue();
        }

        // [Fact]
        // public async Task DroneSimReturnDroneRecordStringInitRegistration()
        // {
        //     var mockedGateway = new Mock<IDroneToDispatcherGateway>();
        //     mockedGateway.Setup(x => x.PostInitialStatus(It.IsAny<DroneStatusUpdateRequest>())).Returns(Task.FromResult(Constants.TestRecord.ToString()));
        //     
        //     var sim = new SimDroneController();
        //     var response = await sim.InitializeRegistration(Constants.TestInitDroneRequest);
        //     response.Should().NotBeNull();
        //     response.Should().NotBeEquivalentTo(Constants.TestRecord.ToString());
        // } No longer exists
        // TODO: untestable, instantiates a gateway and overwrites any mocking
        // [Fact]
        // public async Task DroneSimShouldReturnOkStartDrone()
        // {
        //     var mockedDroneGatewaySetup = new Mock<IDroneToDispatcherGateway>();
        //     mockedDroneGatewaySetup.Setup(x => x.PatchDroneStatus(It.IsAny<DroneStatusUpdateRequest>()))
        //         .Returns(Task.FromResult(Constants.TestRecord.ToString()));
        //     var ExpectedHttp = new OkResult();
        //     var sim = new SimDroneController();
        //     var response = await sim.StartDrone(new Drone(Constants.TestRecord,
        //         mockedDroneGatewaySetup.Object as DroneToDispatchGateway));
        //     response.Should().NotBeNull();
        //     response.Should().NotBeEquivalentTo(ExpectedHttp);
        // } No longer exists

        // [Fact]
        // public async Task TelloAdapterShouldReturnOkAssignToFleet()
        // {
        //     var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
        //     var mockedDroneGatewaySetup = new Mock<IDroneToDispatcherGateway>();
        //     mockedDroneGatewaySetup.Setup(x => x.PatchDroneStatus(It.IsAny<DroneStatusUpdateRequest>()))
        //         .Returns(Task.FromResult(Constants.TestRecord.ToString()));
        //     var sim = new SimDroneController();
        //     var response = await sim.AssignToFleet(Constants.TestCompleteRegistrationResponse);
        //     response.Should().NotBeNull();
        //     response.Should().NotBeEquivalentTo(ExpectedHttp);
        // } NO longer exists

    }
}