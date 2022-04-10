using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;

namespace Domain.InterfaceDefinitions.Gateways
{
    public interface IDroneToDispatcherGateway: IBaseGateway<BaseDTO>
    {
        public Task<bool> PatchDroneStatus(DroneStatusUpdateRequest state);
    }
}
