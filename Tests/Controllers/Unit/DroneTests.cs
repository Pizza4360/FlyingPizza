using System;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Implementation.Gateways;
using Domain.Interfaces.Gateways;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Drone.Tests.Controllers.Unit
{
    public class DroneTests
    {
        // Helper method for console output during testing.
        private readonly ITestOutputHelper _testOutputHelper;

        public DroneTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void drone_should_have_destination_in_route()
        {
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = 3.0m,
                Longitude = 4.0m
            };
            var mockedDispatcherGateway = new Mock<DispatcherGateway>().Object;
            var testDrone = new DroneSimulator.Drone("test_badge", home, mockedDispatcherGateway, new Guid(), "testIp", "testurl");
            testDrone.Destination = dest;

            var route =  testDrone.GetRoute();
            route.Should().NotBeNull();
            route.Should().Contain(dest);
        }
        
        [Fact]
        public void drone_should_have_start_in_route()
        {
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = 3.0m,
                Longitude = 4.0m
            };
            var mockedDispatcherGateway = new Mock<DispatcherGateway>().Object;
            var testDrone = new DroneSimulator.Drone("test_badge", home, mockedDispatcherGateway, new Guid(), "testIp", "testurl");
            testDrone.Destination = dest;

            var route =  testDrone.GetRoute();
            route.Should().NotBeNull();
            route.Should().Contain(home);
        }
        
        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = 3.0m,
                Longitude = 4.0m
            };
            var mockedDispatcherGateway = new Mock<DispatcherGateway>().Object;
            var testDrone = new DroneSimulator.Drone("test_badge", home, mockedDispatcherGateway, new Guid(), "testIp", "testurl");
            testDrone.Destination = dest;
           
            var route =  testDrone.GetRoute();
            route.Should().NotBeNull();
            foreach (var geoLocation in route)
            {
                geoLocation.Latitude.Should().BeGreaterThanOrEqualTo(0m);
                geoLocation.Longitude.Should().BeGreaterThanOrEqualTo(0m);
            }
        }

        [Fact]
        public void TestGetRouteAllNegativeNumbers()
        {
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = -3.0m,
                Longitude = -4.0m
            };
            var mockedDispatcherGateway = new Mock<DispatcherGateway>().Object;
            var testDrone = new DroneSimulator.Drone("test_badge", home, mockedDispatcherGateway, new Guid(), "testIp", "testurl");
            testDrone.Destination = dest;

            var route =  testDrone.GetRoute();
            route.Should().NotBeNull();
            foreach (var geoLocation in route)
            {
                geoLocation.Latitude.Should().BeLessThanOrEqualTo(0.0m);
                geoLocation.Longitude.Should().BeLessThanOrEqualTo(0.0m);
            }
        }
    }
}
