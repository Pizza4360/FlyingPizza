using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using MongoDB.Driver;

namespace Domain.InterfaceDefinitions.Repositories;

public interface IFleetRepository : IBaseRepository<DroneRecord>
{
    public Task<Dictionary<string, string>> GetAllAddresses();
    public Task<UpdateResult> PatchDroneStatus(DroneStatusUpdateRequest stateDto);

    public Task<DroneRecord> GetAsync(string id);
}