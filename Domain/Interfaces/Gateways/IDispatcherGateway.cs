using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;

namespace Domain.Interfaces.Gateways
{
    public interface IDispatcherGateway
    {
        public Task<bool> PutDroneState(UpdateStatusDto status);
    }
}
