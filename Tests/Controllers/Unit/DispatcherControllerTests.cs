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

        // TODO: functions on sim, not drones
        // [Fact]
        // public async Task TelloAdapterShouldReturnOkInitRegistration()
        // {
        //     var adapter = new Drone(Constants.TestRecord, new DroneToDispatchGateway());
        //     var response = await adapter.InitializeRegistration(Constants.TestInitDroneRequest);
        //     response.Should().NotBeNull();
        //     response.Should().NotBeEquivalentTo(ExpectedHttp);
        // }

        // [Fact]
        // public async Task TelloAdapterShouldReturnOkStartDrone()
        // {
        //     var adapter = new Drone(Constants.TestRecord, new DroneToDispatchGateway());
        //     var response = await adapter.StartDrone(new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway()));
        //     response.Should().NotBeNull();
        //     response.Should().NotBeEquivalentTo(ExpectedHttp);
        // }

        // [Fact]
        // public async Task TelloAdapterShouldReturnOkAssignToFleet()
        // {
        //     var adapter = new Drone(Constants.TestRecord, new DroneToDispatchGateway());
        //     adapter._drone = new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway());
        //     var response = await adapter.AssignToFleet(Constants.TestCompleteRegistrationResponse);
        //     response.Should().NotBeNull();
        //     response.Should().NotBeEquivalentTo(ExpectedHttp);
        // }

        [Fact]
        public void drone_should_have_destination_in_route()
        {
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
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
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
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
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