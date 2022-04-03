using System.Threading.Tasks;
using Domain.DTO.DispatcherDrone.DroneToDispatcher;

namespace Domain.Interfaces.Gateways
{
    public interface IDispatcherGateway
    {
        public Task<bool> PutDroneState(PutStatusDto status);
    }
}
