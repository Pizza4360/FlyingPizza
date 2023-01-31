using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;

namespace Tests;

public class Constants
{
    public const string DroneIp = "localhost:5000";
    public const string DispatcherIp = "localhost:4000";
    public const string Url = "https://FlyingPizza.com";
    public const string DroneId = "abcdefabcdefabcdefabcdef";
    public const string TestDeliveryId = "123456123456123456123456";
    public const string CHARGING = "Charging";
    public static readonly object[] TestItems = Array.Empty<object>();
    public static readonly Deliveriestatus TestDeliveriestatus = Deliveriestatus.Waiting;
    
    private static readonly GeoLocation
        TestDeliveryLocation = new()
        {
            Latitude = 39.74362771992734m, Longitude = -105.00549345883957m
        },
        TestHomeLocation = new()
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00561147600774m
        };

    private const string
        TestDeliveryAddress = "yo mama",
        TestCustomerName = "bobby";

    public static readonly GeoLocation HomeLocation = new()
    {
        Latitude = 0.00001m,
        Longitude = 0.00001m
    };

    public static readonly GeoLocation Destination = new()
    {
        Latitude = 0.00018m,
        Longitude = 0.00011m
    };
    private static readonly DateTime TestTimeDelivered = DateTime.UtcNow;

    
    public static DroneEntity Entity = new()
    {
        DispatchUrl = Url,
        Id = DroneId,
        DroneUrl = DispatcherIp,
        HomeLocation = HomeLocation,
        Destination = Destination,
        CurrentLocation = HomeLocation,
        DeliveryId = TestDeliveryId,
        LatestStatus = DroneStatus.Ready,
        DroneId = DroneId
    };
    public static readonly DeliveryEntity TestDeliveryEntity = new()
    {
        CustomerName = TestCustomerName,
        DeliveryAddress = TestDeliveryAddress,
        DeliveryLocation = TestDeliveryLocation,
        DroneId = Entity.DroneId,
        Id = TestDeliveryId,
        Items = TestItems,
        DeliveryId = TestDeliveryId,
        Status = TestDeliveriestatus,
        TimeDelivered = TestTimeDelivered,
        TimeDeliveryed = TestTimeDelivered
        
    };

    public static readonly Dictionary<string, string> TestStringDict = new()
    {
        ["something"] = "something"
    };

    public static readonly HttpResponseMessage Httpok = new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent("")
    };
    
    public static readonly CompleteDeliveryRequest TestCompleteDeliveryRequest = new()
    {
        DeliveryId = TestDeliveryId,
        Time = TestTimeDelivered
    };
    public static string TelloIp = "192.168.10.1";
    
        public static readonly AssignDeliveryRequest TestAssignDeliverRequest = new()
        {
            DroneId = DroneId,
            DeliveryId = TestDeliveryId,
            DeliveryLocation = Destination
        };

        public static readonly AssignDeliveryResponse TestAssignDeliveryResponse = new()
        {
            DroneId = DroneId,
            DeliveryId = TestDeliveryId,
            Success = true
        };

        public static readonly UpdateDroneStatusResponse TestUpdateResponse = new()
        {
            DroneId = DroneId,
            IsCompletedSuccessfully = true
        };
        public static string TestPingString = "I am pinging!";
        
        public static BaseDto TestPingDto = new()
        {
            Message = TestPingString
        };
        
        public static readonly AddDroneRequest TestAddDroneRequest = new()
        {
            DispatchUrl = Entity.DispatchUrl,
            DroneId = Entity.DroneId,
            DroneUrl = Entity.DroneUrl,
            HomeLocation = Entity.HomeLocation
        };

        public static readonly AddDroneResponse TestAddDroneResponse = new()
        {
            DroneId = Entity.DroneId,
            Success = true
        };

        public static readonly InitDroneResponse TestInitDroneResponse = new()
        {
            DroneId = Entity.DroneId,
            Okay = true
        };

        public static readonly AssignFleetResponse TestAssignFleetResponse = new()
        {
            DroneId = Entity.DroneId,
            FirstStatus = DroneStatus.Ready,
            IsInitializedAndAssigned = true

        };

        public static readonly EnqueueDeliveryRequest TestEnqueueDeliveryRequest = new()
        {
            DeliveryId = TestDeliveryId,
            DeliveryLocation = TestDeliveryEntity.DeliveryLocation
        };
        
        public static readonly EnqueueDeliveryResponse TestEnqueueDeliveryResponse = new()
        {
            IsAssigned = true
        };

        public static readonly InitDroneRequest TestInitDroneRequest = new()
        {
            DroneId = Entity.DroneId,
            DroneUrl = Entity.DroneUrl
        };

        public static readonly AssignFleetRequest TestAssignFleetRequest = new()
        {
            DispatchUrl = Entity.DispatchUrl,
            DroneId = Entity.DroneId,
            DroneUrl = Entity.DroneUrl,
            HomeLocation = Entity.HomeLocation
        };

        public static readonly CompleteDeliveryResponse TestCompleteDeliveryResponse = new()
        {
            IsAcknowledged = true
        };
    

        public static DroneUpdate TestUpdate = new()
        {
            CurrentLocation = Entity.CurrentLocation,
            Destination = Entity.Destination,
            DroneId = Entity.DroneId,
            DeliveryId = Entity.DeliveryId,
            Status = Entity.LatestStatus
        };

        public static readonly DroneEntity TestEntityDead = new()
        {
            CurrentLocation = Entity.CurrentLocation,
            Destination = Entity.Destination,
            DispatchUrl = Entity.DispatchUrl,
            DroneId = Entity.DroneId,
            DroneUrl = Entity.DroneUrl,
            HomeLocation = Entity.HomeLocation,
            Id = Entity.Id,
            DeliveryId = Entity.DeliveryId,
            LatestStatus = DroneStatus.Dead
        };

        public static readonly RecoveryRequest TestReviveRequest = new()
        {
            Entity = TestEntityDead
        };

        public static readonly DroneEntity TestEntityReturnHome = new()
        {
            CurrentLocation = Entity.CurrentLocation,
            Destination = Entity.Destination,
            DispatchUrl = Entity.DispatchUrl,
            DroneId = Entity.DroneId,
            DroneUrl = Entity.DroneUrl,
            HomeLocation = Entity.HomeLocation,
            Id = Entity.Id,
            DeliveryId = Entity.DeliveryId,
            LatestStatus = DroneStatus.Returning
        };
}