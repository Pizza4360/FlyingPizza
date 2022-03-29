﻿using System;
using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces.Gateways
{
    public interface IDroneGateway
    {
        public Task<bool> StartRegistration(string droneIpAddress, Guid badgeNumber, string 
        dispatcherUrl, GeoLocation homeLocation);

        public Task<bool> AssignDelivery(string droneIpAddress, string orderNumber, GeoLocation orderLocation);

        public Task<bool> OKToSendStatus(string droneIpAddress);
    }
}