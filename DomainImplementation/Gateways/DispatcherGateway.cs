using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.Interfaces.Gateways;

namespace Domain.Implementation.Gateways
{
    public class DispatcherGateway : IDispatcherGateway
    {
        public Task<bool> UpdateDroneStatus(UpdateStatusDto status)
        {
            throw new System.NotImplementedException();
        }
    }
}
