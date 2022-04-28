﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.RepositoryDefinitions;

public interface IFleetRepository : IBaseRepository<DroneRecord>
{
}