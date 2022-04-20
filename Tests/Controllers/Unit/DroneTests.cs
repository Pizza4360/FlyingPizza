using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.InterfaceDefinitions.Gateways;
using Domain.InterfaceImplementations.Gateways;
using FluentAssertions;
using Moq;
using SimDrone;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Controllers.Unit;

public class DroneTests
{
    // Helper method for console output during testing.
        private readonly ITestOutputHelper _testOutputHelper;

        public DroneTests(
        ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

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
            var drone = new Drone(Constants.TestRecord, new DroneToDispatchGateway());
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