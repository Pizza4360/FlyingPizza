﻿using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces.Gateways
{
    public interface IDroneGateway
    {
        public Task<bool> CompleteRegistration(string droneIpAddress, string droneId, string dispatcherUrl, GeoLocation homeLocation);

        public Task<bool> AssignDeilvery(string droneIpAddress, string orderNumber, GeoLocation orderLocation);
    }
}
