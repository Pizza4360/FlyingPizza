using System;
using Domain.DTO;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
using FluentAssertions;
using SimDrone;
using Xunit;

namespace Tests.Controllers.Unit;

public class RealDroneTests
{
    // DO NOT UNCOMMENT THIS UNLESS WIFI INTO TELLO SDK DRONE
    
    // [Fact]
    // public void imScared()
    // {
    //     var testGateway = new DroneToDispatchGateway();
    //     var myTable = new GeoLocation
    //     {
    //         Latitude = 0.0m,
    //         Longitude = 0.0m
    //     };
    //     var otherTable = new GeoLocation
    //     {
    //         // 1 meter of travel
    //         Latitude = 0.001m,
    //         Longitude = 0.0m
    //     };
    //     var droneRecord = new DroneRecord
    //     {
    //         DroneId = new Guid(),
    //         CurrentLocation = myTable,
    //         Destination = otherTable,
    //         DispatcherUrl = Constants.DispatcherIp,
    //         DroneIp = Constants.TelloIp,
    //         HomeLocation = myTable,
    //         OrderId = Constants.DroneUrl,
    //         Orders = Constants.TestOrderId,
    //         State = DroneState.Ready
    //     };
    //     var testTello = new TelloDrone(droneRecord, testGateway, false);
    //     var result = testTello.DeliverOrder(otherTable);
    //     result.Should().BeTrue();
    }
}