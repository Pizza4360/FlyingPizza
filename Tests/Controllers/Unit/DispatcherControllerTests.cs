using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DomainImplementation.Repositories;
using DroneDispatcher.Controllers;

namespace Drone.Tests.Controllers.Unit
{


    public class DispatcherControllerTests
    {

        public Order getFakeOrder()
        {
            var testLocation = new GeoLocation
            {
                Latitude = 39.74362771992734m, Longitude = -105.00549345883957m,
            };
            return new Order
            {
                DeliveryAddress = "yo mama",
                TimeDelivered = DateTimeOffset.UtcNow,
                Id = "some stuff",
                TimeOrdered = DateTimeOffset.UtcNow,
                DeliveryLocation = testLocation,
                CustomerName = "bobby"
            };
        }

        // Update Status
        [Fact]
        public async Task dispatcher_controller_should_return_ok()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            var mockedDatabase = new Mock<IMongoDatabase>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var controller =
                new DispatcherController(new DronesRepository(mockedDatabase, "bogus"), mockedOrdersRepo, mockedDroneGateway);
            var result = await controller.UpdateStatus(new UpdateStatusDto());
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);

        }

        // RegisterNewDrone
        [Fact]
        public async Task dispatcher_controller_register_should_send_proper_data_to_gateway()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
            var testGuid = new Guid();
            // Here we set up a verify hook so we can see if the right arguments were used in a startRegistration call
            mockedDroneGatewaySetup.Setup(x => x.StartRegistration("test_ip", testGuid, "http://172.18.0.0:4000",
                new GeoLocation
                {
                    Latitude = 39.74364421910773m,
                    Longitude = -105.00561147600774m
                })).Returns(Task.FromResult(true)).Verifiable();

            var mockedDroneGateway = mockedDroneGatewaySetup.Object;

            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var mockedDroneRepositorySetup = new Mock<IDronesRepository>();
            mockedDroneRepositorySetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>())).Returns<Domain.Entities.DroneRecord>(x =>Task.FromResult(x));
            var mockedDroneRepository = mockedDroneRepositorySetup.Object;
            var controller =
                new DispatcherController(mockedDroneRepository,mockedOrdersRepo, mockedDroneGateway);
            await controller.RegisterNewDrone(testInfo);
            mockedDroneGatewaySetup.VerifyAll();
        }

        [Fact]
        public async Task dispatcher_controller_should_return_problem_on_incorrect_drone_info()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            var mockedDatabase = new Mock<IMongoDatabase>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var testGuid = new Guid();

            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller =
                new DispatcherController(new DronesRepository(mockedDatabase, "bogus"), mockedOrdersRepo,mockedDroneGateway);
            var result = await controller.RegisterNewDrone(testInfo);
            // Problem object result used in register drone that is invalid for now, will change with black box changes.
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task dispatcher_should_return_ok_on_valid_drone_info()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            var mockedDronesRepositorySetup = new Mock<IDronesRepository>();
            // Forcing mongo mock to accept non-existent drone without connecting to server
            mockedDronesRepositorySetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.DroneRecord>()))
                .Returns<Domain.Entities.DroneRecord>((x => Task.FromResult(x)));
            var mockedDronesRepository = mockedDronesRepositorySetup.Object;
            // Forcing mock of gateway to say drone is valid
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
            mockedDroneGatewaySetup
                .Setup(x => x.StartRegistration(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<GeoLocation>())).Returns(Task.FromResult(true));
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            var testGuid = new Guid();

            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller = new DispatcherController(mockedDronesRepository,mockedOrdersRepo, mockedDroneGateway);
            var result = await controller.RegisterNewDrone(testInfo);
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }


        // addNewOrder
        [Fact]
        public async Task add_new_order_should_assign_an_order_if_available()
        {
            var mockedOrdersRepoSetup = new Mock<IOrdersRepository>();
            var mockedDronesRepositorySetup = new Mock<IDronesRepository>();
            var testLocation = new GeoLocation
            {
                Latitude = 39.74362771992734m, Longitude = -105.00549345883957m,
            };
            var testDestination = new GeoLocation
            {
                Latitude = 39.74313274570401m, Longitude = -105.00641613869328m
            };
            var testGuid = new Guid();
            mockedDronesRepositorySetup.Setup(x => x.GetAllAvailableDronesAsync()).Returns(Task.FromResult(new List<Domain.Entities.DroneRecord>(1){new Domain.Entities.DroneRecord
            {
                IpAddress = "test_ip",
                Destination = testDestination,
                BadgeNumber = testGuid,
                CurrentLocation = testLocation,
                State = "on fire",
                OrderId = "good enough",
                DispatcherUrl = "test_url",
                Id = "some stuff",
                HomeLocation = testLocation
            }} as IEnumerable<Domain.Entities.DroneRecord>));
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
            mockedDronesRepositorySetup.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Domain.Entities.DroneRecord()));
            var mockedDronesRepository = mockedDronesRepositorySetup.Object;
            
            mockedOrdersRepoSetup.Setup(x => x.GetByIdAsync("some stuff")).Returns(Task.FromResult(getFakeOrder()));
            var mockedOrdersRepo = mockedOrdersRepoSetup.Object;
            mockedDroneGatewaySetup
                .Setup(x => x.AssignDelivery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>()))
                .Returns(Task.FromResult(true)).Verifiable();
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller = new DispatcherController(mockedDronesRepository, mockedOrdersRepo,mockedDroneGateway);
            var testOrderDto = new AddOrderDTO
            {
                DeliveryLocaion = testDestination,
                Id = "some stuff"
            };
            
            // calling method
            await controller.AddNewOrder(testOrderDto);

            mockedDroneGatewaySetup.VerifyAll();
        }

        [Fact]
        public async Task add_new_order_should_return_ok()
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            mockedDroneGatewaySetup
                .Setup(x => x.AssignDelivery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>()))
                .Returns(Task.FromResult(true)).Verifiable();
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            var testGuid = new Guid();
            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller = new DispatcherController(mockedDronesRepository,mockedOrdersRepo, mockedDroneGateway);
            var testOrderDto = new AddOrderDTO();
            // calling method
            var result = await controller.AddNewOrder(testOrderDto);

            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        // CompleteDelivery
        [Fact]
        public async Task complete_delivery_should_return_ok()
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var mockedOrdersRepositorySetup = new Mock<IOrdersRepository>();
            var testOrderNumber = "35";
            var testOrder = new Order
            {
                Id = testOrderNumber,
                CustomerName = "testee",
                DeliveryLocation = new GeoLocation
                {
                    Latitude = 69,
                    Longitude = 69
                },
                TimeOrdered = DateTimeOffset.UtcNow
                // TimeDelivered assumed null by business logic
            };
 
            // Ensuring orders repo is updated
            mockedOrdersRepositorySetup.Setup(x => x.GetByIdAsync(testOrderNumber)).Returns(Task.FromResult(testOrder)).Verifiable();
            mockedOrdersRepositorySetup.Setup(x => x.Update(testOrder)).Returns(Task.FromResult(testOrder)).Verifiable();
            var mockedOrdersRepository = mockedOrdersRepositorySetup.Object;
            // TODO: BUG # 2 currently fails since Dev hasn't added order repo to dispatcher
            
            var controller = new DispatcherController(mockedDronesRepository, mockedOrdersRepository, mockedDroneGateway);
            // calling method
            var result = await controller.CompleteDelivery(testOrderNumber);

            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
            mockedOrdersRepositorySetup.VerifyAll();
        }
        
        [Fact]
        public async Task complete_delivery_should_update_orders()
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var mockedOrdersRepositorySetup = new Mock<IOrdersRepository>();
            var testOrderNumber = "35";
            var testOrder = new Order
            {
                Id = testOrderNumber,
                CustomerName = "testee",
                DeliveryLocation = new GeoLocation
                {
                    Latitude = 69,
                    Longitude = 69
                },
                TimeOrdered = DateTimeOffset.UtcNow
                // TimeDelivered assumed null by business logic
            };
 
            // Ensuring orders repo is updated
            mockedOrdersRepositorySetup.Setup(x => x.GetByIdAsync(testOrderNumber)).Returns(Task.FromResult(testOrder)).Verifiable();
            mockedOrdersRepositorySetup.Setup(x => x.Update(testOrder)).Returns(Task.FromResult(testOrder)).Verifiable();
            var mockedOrdersRepository = mockedOrdersRepositorySetup.Object;
            // TODO:BUG #2 currently fails since Dev hasn't added order repo to dispatcher
            
            var controller = new DispatcherController(mockedDronesRepository, mockedOrdersRepository,mockedDroneGateway);
            // calling method
            await controller.CompleteDelivery(testOrderNumber);

            mockedOrdersRepositorySetup.VerifyAll();
        }

        // DroneIsReadyForOrders
        [Fact]
        public async Task drone_is_ready_for_orders_should_always_return_ok()
        {
            var mockedOrdersRepo = new Mock<IOrdersRepository>().Object;
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var testGuidString = "invalid in every way";
            var controller = new DispatcherController(mockedDronesRepository,mockedOrdersRepo ,mockedDroneGateway);
            // calling method
            var result = await controller.DroneIsReadyForOrder(testGuidString);

            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public async Task drone_is_ready_for_orders_should_assign_the_correct_order()
        {
            var testGuidString = "close enough";
            var testLocation = new GeoLocation
            {
                Latitude = 69,
                Longitude = 69
            };
            var testOrderDto = new AddOrderDTO
            {
                DeliveryLocaion = testLocation,
                Id = testGuidString
            };
  
            //_ordersRepository.GetByIdAsync
            var mockedOrdersRepoSetup = new Mock<IOrdersRepository>();
                mockedOrdersRepoSetup.Setup(x => x.GetByIdAsync(testGuidString)).Returns(Task.FromResult(new Order
            {
                DeliveryAddress = "yo mama",
                TimeDelivered = DateTimeOffset.UtcNow,
                Id = testGuidString,
                TimeOrdered = DateTimeOffset.UtcNow,
                DeliveryLocation = testLocation,
                CustomerName = "bobby"
            }));
            var mockedOrdersRepo = mockedOrdersRepoSetup.Object;
            var mockedDronesRepositorySetup = new Mock<IDronesRepository>();
            var mockedDroneGatewaySetup  = new Mock<IDroneGateway>();
            mockedDroneGatewaySetup.Setup(x => x.AssignDelivery(It.IsAny<string>(),testGuidString, testLocation)).Verifiable();
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            var mockedDispatcherGateway = new Mock<IDispatcherGateway>().Object;

            mockedDronesRepositorySetup.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Domain.Entities.DroneRecord()));
            var mockedDronesRepository = mockedDronesRepositorySetup.Object;

            
            // Loading up with test order
            
            
            
            var controller = new DispatcherController(mockedDronesRepository,mockedOrdersRepo ,mockedDroneGateway);
            
            // need a drone available to add an order?
            
            // adding wanted order to orders
            await controller.AddNewOrder(testOrderDto);
            
            // calling method
            await controller.DroneIsReadyForOrder(testGuidString);

            // Checking assign has right parameters
            mockedDroneGatewaySetup.VerifyAll();
        }
    
        [Fact]
        public async Task drone_is_ready_for_orders_should_get_async_correctly()
        {
            var mockedDronesRepositorySetup = new Mock<IDronesRepository>();
            mockedDronesRepositorySetup.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Verifiable();

            var testGuidString = "close enough";
            var testLocation = new GeoLocation
            {
                Latitude = 69,
                Longitude = 69
            };
            var testOrderDto = new AddOrderDTO
            {
                DeliveryLocaion = testLocation,
                Id = testGuidString
            };
            
            
            mockedDronesRepositorySetup.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Domain.Entities.DroneRecord()));
;
            var mockedDronesRepository = mockedDronesRepositorySetup.Object;
            var mockedDroneGatewaySetup  = new Mock<IDroneGateway>();
            
            mockedDroneGatewaySetup.Setup(x => x.AssignDelivery(It.IsAny<string>(),testGuidString, testLocation)).Verifiable();
            
            var mockedOrdersRepoSetup = new Mock<IOrdersRepository>();
            mockedOrdersRepoSetup.Setup(x => x.GetByIdAsync(testGuidString)).Returns(Task.FromResult(new Order
            {
                DeliveryAddress = "yo mama",
                TimeDelivered = DateTimeOffset.UtcNow,
                Id = testGuidString,
                TimeOrdered = DateTimeOffset.UtcNow,
                DeliveryLocation = testLocation,
                CustomerName = "bobby"
            }));
            var mockedOrdersRepo = mockedOrdersRepoSetup.Object;
            // Loading up with test order
            
            
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            
            var controller = new DispatcherController(mockedDronesRepository,mockedOrdersRepo ,mockedDroneGateway);
            
            
            // adding wanted order to orders
            await controller.AddNewOrder(testOrderDto);
            
            // calling method
            await controller.DroneIsReadyForOrder(testGuidString);

            // Checking getasync has right parameters
            mockedDroneGatewaySetup.VerifyAll();
        }
}
}