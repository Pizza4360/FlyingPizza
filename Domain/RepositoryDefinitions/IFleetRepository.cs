﻿using Domain.DTO;
using Domain.Entities;

namespace Domain.RepositoryDefinitions;

public interface IFleetRepository : IBaseRepository<DroneRecord, DroneUpdate>
{
}