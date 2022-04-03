using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DroneDispatcher.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
namespace DroneTests.Controllers
{
    public class Utils
    {
        
        public const string
            TestDeliveryAddress = "yo mama",
            TestOrderNumber = "123",
            TestCustomerName = "bobby",
            TestOrderId = "some stuff",
            TestInvalidIp = "test_ip",
            TestValidIp = "172.18.0.0",
            TestDispatcherUrl = "http://" + TestValidIp + ":4000";
           
        public static readonly GeoLocation 
            TestDeliveryLocation = new() 
            {
                Latitude = 39.74362771992734m, Longitude = -105.00549345883957m
            },
            TestHomeLocation = new()
            {
                Latitude = 39.74364421910773m,
                Longitude = -105.00561147600774m
            };
        
        public static readonly Guid TestGuid = new();
        public static readonly DateTimeOffset TestTimeDelivered = DateTimeOffset.UtcNow;
        
        public static readonly Order TestOrder = new()
        {
            DeliveryAddress = TestDeliveryAddress,
            TimeDelivered = TestTimeDelivered,
            Id = TestOrderNumber,
            TimeOrdered = TestTimeDelivered,
            DeliveryLocation = TestDeliveryLocation,
            CustomerName = TestCustomerName
        };
        public static readonly PostDroneRegistrationInfoDto 
            BadPostDroneInfoDto = new()
            {
                BadgeNumber = TestGuid,
                IpAddress = TestInvalidIp
            }, 
            PostDroneRegistrationInfoDto = new() {
                BadgeNumber = TestGuid,
                IpAddress = TestValidIp
            };

        public static readonly AddOrderDTO AddOrderDto = new()
        {
            DeliveryLocaion = TestDeliveryLocation,
            Id = TestOrderId
        };
        public static readonly Domain.Entities.Drone TestDrone = new() {
            IpAddress = TestValidIp,
            Destination = TestDeliveryLocation,
            BadgeNumber = TestGuid,
            CurrentLocation = TestHomeLocation,
            Status = Domain.Constants.DroneStatus.READY,
            OrderId = TestOrderId,
            DispatcherUrl = TestDispatcherUrl,
            Id = "TestGuid",
            HomeLocation = TestHomeLocation
        };
    }
}