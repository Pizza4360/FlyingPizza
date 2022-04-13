using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dispatch.Controllers;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using SimDrone;

namespace Tests
{
    public class Constants
    {
        public const string DroneIp = "localhost:5000";
        public const string DispatcherIp = "localhost:4000";
        public const string Url = "https://FlyingPizza.com";
        public static readonly Guid TestBadgeNumber = Guid.NewGuid();
        public const string DroneId = "abcdefg";
        public const string TestOrderId = "123456";
        
        public static readonly GeoLocation HomeLocation = new()
        {
            Latitude = 0.00001m,
            Longitude = 0.00001m
        };
        public static readonly GeoLocation Destination = new()
        {
            Latitude = 0.00008m,
            Longitude = 0.00001m
        };
        public const string CHARGING = "Charging";

        public static DroneRecord TestRecord = new()
        {
            BadgeNumber = TestBadgeNumber,
            DispatcherUrl = Url,
            Id = DroneId,
            IpAddress = DispatcherIp,
            HomeLocation = HomeLocation,
            Destination = Destination,
            CurrentLocation = HomeLocation,
            OrderId  = TestOrderId,
            State = DroneState.Ready
        };
        
           private const string
            TestDeliveryAddress = "yo mama",
            TestCustomerName = "bobby",
            InvalidTestIp = "test_ip",
            ValidTestIp = "172.18.0.0",
            TestDispatcherUrl = "http://" + ValidTestIp + ":4000";
            
           
           
           public static readonly Dictionary<string, string> TestStringDict = new()
           {
               ["something"] = "something"
           };
           
           public static readonly InitDrone TestInitDroneDto = new()
           {
               FistStatusUpdateRequestUpdate = TestDroneStatusUpdateRequest,
               Record = TestRecord
           };

           public static readonly HttpResponseMessage Httpok = new()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent("")
           };
           
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
        public static readonly DateTime TestTimeDelivered = DateTime.UtcNow;
        
        public static readonly Order TestOrder = new()
        {
            DeliveryAddress = TestDeliveryAddress,
            TimeDelivered = TestTimeDelivered,
            Id = TestOrderId,
            TimeOrdered = TestTimeDelivered,
            DeliveryLocation = TestDeliveryLocation,
            CustomerName = TestCustomerName
        };

        public static readonly
            Domain.DTO.FrontEndDispatchCommunication.AddDroneRequest
            BadDroneRequest = new()
            {
                BadgeNumber = TestBadgeNumber,
                DispatchIp = InvalidTestIp
            },
            DroneRegistrationInfo = new()
            {
                BadgeNumber = TestBadgeNumber,
                DispatchIp = ValidTestIp,
                Id = DroneId,
                DroneIp = DroneIp,
                HomeLocation = HomeLocation
            };

        public static AddOrderResponse AddOrderResponse = new()
        {
            
        };
        public static Domain.DTO.FrontEndDispatchCommunication.AddOrderRequest
        addOrderRequest= new()
        {
            OrderId = TestOrderId,
            
        };
        public static readonly GatewayDto TestGatewayDto = new ();
        
        public static readonly CompleteOrderRequest TestCompleteOrderRequest = new()
        {
            OrderId = TestOrderId,
            Time = TestTimeDelivered
        };
        public static readonly DroneStatusUpdateRequest TestDroneStatusUpdateRequest = new()
        {
            Id = TestGuid.ToString(),
            Location = TestDeliveryLocation,
            State = DroneState.Ready
        };

        public static InitDroneResponse TestInitRegistrationResponse
        {
            get;
            set;
        } = new ()
        {
            Id = DroneId,
            Okay = true
        };

    }
}
        

