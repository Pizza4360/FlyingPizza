using System;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Implementation.Gateways;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DroneDispatcher.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using DroneTests;

using DroneUtils;


namespace DroneTests.Controllers.Unit
{
    public class DroneControllerTests
    {
        [Fact]
        public async Task DronesAreEqual()
        {
            var droneBlueprint = DroneUtils.Utils.TestDrone;
            DroneSimulator.Drone drone1 = new DroneSimulator.Drone(
                droneBlueprint.Id, droneBlueprint.HomeLocation, 
                new Mock<IDispatcherGateway>().Object
                );

        }
        
        [Fact]
        public async Task DroneHasCorrectFieldsAfterInstantiation()
        {
            // Make a drone object
        }

        [HttpPost("initregistration")]
        public async Task<IActionResult> InitializeRegistration()
        {
           
        }

        [HttpPost("completeregistration")]
        public async Task<IActionResult> CompleteRegistration()
        {
            
        }

        public void changeDrone(Drone drone)
        {
           
        }
    }
}
