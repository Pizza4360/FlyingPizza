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
        public async Task DroneSimShouldReturnTrue()
        {
            // Assumed to return an ok object result with ok as arg
            var adapter = new Drone(Constants.TestRecord, new DroneToDispatchGateway());
            var response = adapter.DeliverOrder(Constants.TestOrder.DeliveryLocation);
            response.Should().BeTrue();
        }
        [Fact]
        public async Task DroneSimReturnDroneRecordStringInitRegistration()
        {
            var sim = new SimDroneController();
            var response = await sim.InitializeRegistration(Constants.TestInitDroneRequest);
            response.Should().NotBeNull();
            response.Should().NotBeEquivalentTo(Constants.TestRecord.ToString());
        }

        [Fact]
        public async Task DroneSimShouldReturnOkStartDrone()
        {
            var mockedDroneGatewaySetup = new Mock<IDroneToDispatcherGateway>();
            mockedDroneGatewaySetup.Setup(x => x.PatchDroneStatus(It.IsAny<DroneStatusUpdateRequest>()))
                .Returns(Task.FromResult(Constants.TestRecord.ToString()));
            var ExpectedHttp = new OkResult();
            var sim = new SimDroneController();
            var response = await sim.StartDrone(new Drone(Constants.TestRecord, mockedDroneGatewaySetup.Object as DroneToDispatchGateway));
            response.Should().NotBeNull();
            response.Should().NotBeEquivalentTo(ExpectedHttp);
        }

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

        [Fact]
        public void drone_should_have_destination_in_route()
        {
            //TODO: Bug #5 drone moves in Latitude and Longitude direction indefinitely
            var drone = new Drone(Constants.TestRecord, new DroneToDispatchGateway());
            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            route.Should()
                .ContainEquivalentOf(Constants.Destination);
        }

        [Fact]
        public void drone_should_have_start_in_route()
        {
            
            //TODO: Bug #5 drone moves in Latitude and Longitude direction indefinitely
            var drone = new Drone(Constants.TestRecord, new DroneToDispatchGateway());
            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            route.Should()
                .ContainEquivalentOf(Constants.HomeLocation);
        }

        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            
            //TODO: Bug #5 drone moves in Latitude and Longitude direction indefinitely
            var drone = new Drone(Constants.TestRecord, new DroneToDispatchGateway());
            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            foreach (var geoLocation in route)
            {
                geoLocation.Latitude.Should()
                    .BeGreaterThanOrEqualTo(0m);
                geoLocation.Longitude.Should()
                    .BeGreaterThanOrEqualTo(0m);
            }
        }

        [Fact]
        public void TestGetRouteAllNegativeNumbers()
        {
            
            // Bug #5 drone moves in Latitude and Longitude direction indefinitely
            var mockedDroneGatewaySetup = new Mock<IDroneToDispatcherGateway>();
            mockedDroneGatewaySetup.Setup(x => x.PatchDroneStatus(It.IsAny<DroneStatusUpdateRequest>()))
                .Returns(Task.FromResult(Constants.TestRecord.ToString()));
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            var drone = new Drone(Constants.TestRecord, mockedDroneGateway as DroneToDispatchGateway);
            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            foreach (var geoLocation in route)
            {
                geoLocation.Latitude.Should()
                    .BeLessThanOrEqualTo(0.0m);
                geoLocation.Longitude.Should()
                    .BeLessThanOrEqualTo(0.0m);
            }
        }
    }
}