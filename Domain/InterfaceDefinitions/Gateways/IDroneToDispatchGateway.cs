using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;

namespace Domain.InterfaceDefinitions.Gateways
{
    public interface IDroneToDispatcherGateway: IBaseGateway<BaseDto>
    {
        public Task<string?> PatchDroneStatus(DroneStatusUpdateRequest state);
        public Task<string?> PostInitialStatus(DroneStatusUpdateRequest state);
    }
}
