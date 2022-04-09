using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;

namespace Domain.Interfaces.Gateways
{
    public interface IDispatcherGateway
    {
        public Task<bool> PatchDroneStatus(DroneStatusPatch state);
    }
}
