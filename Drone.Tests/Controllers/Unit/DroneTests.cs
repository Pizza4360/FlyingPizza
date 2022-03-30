using System;
using System.Configuration;
using System.Linq;
using Castle.Core.Internal;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using FlyingPizza.Drone;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using Moq;

namespace Drone.Tests
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
            var home = new Point(0.0, 0.0);
            var dest = new Point(3.0, 4.0);

            var drone = DroneModel.AddDrone("").Result;
            drone.Delivery = dest;

            var route =  drone.GetRoute();
            route.Should().NotBeNull();
            route.Should().Contain(dest);
        }
        
        [Fact]
        public void drone_should_have_start_in_route()
        {
            var mockedDispatcher = new Mock<IDispatcherGateway>().Object;
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = 3.0m,
                Longitude = 4.0m
            };
            var drone = new DroneSimulator.Drone("test id", home, mockedDispatcher);
            drone.Destination = dest;

            var route =  drone.GetRoute();
            route.Should().NotBeNull();
            route.Should().Contain(home);
        }
        
        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            var mockedDispatcher = new Mock<IDispatcherGateway>().Object;
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = 3.0m,
                Longitude = 4.0m
            };
            var drone = new DroneSimulator.Drone("test id", home, mockedDispatcher);
            drone.Destination = dest;

            var route =  drone.GetRoute();
            route.Should().NotBeNull();
            route.Should().Satisfy(x => x.Latitude >= 0.0m, x => x.Longitude >= 0.0m);

        }

        [Fact]
        public void TestGetRouteAllNegativeNumbers()
        {
            var mockedDispatcher = new Mock<IDispatcherGateway>().Object;
            var home = new GeoLocation{
                Latitude = 3.0m,
                Longitude = 4.0m
                
            };
            var dest = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
            };
            var drone = new DroneSimulator.Drone("test id", home, mockedDispatcher);
            drone.Destination = dest;

            var route =  drone.GetRoute();
            route.Should().NotBeNull();
            route.Should().Satisfy(x => x.Latitude <= 0.0m, x => x.Longitude <= 0.0m);
        }
    }
}
