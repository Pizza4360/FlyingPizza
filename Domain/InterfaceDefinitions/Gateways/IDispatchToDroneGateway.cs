using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;

namespace Domain.InterfaceDefinitions.Gateways
{
    public interface IDispatchToDroneGateway : IBaseGateway<BaseDto>
    {
        public Task<HttpResponseMessage> StartRegistration(
            string droneIpAddress);

        public AssignDeliveryResponse AssignDelivery(
            AssignDeliveryRequest request);

        public AssignFleetResponse AssignToFleet(
            AssignFleetRequest assignment);
        
    }
}
