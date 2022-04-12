﻿using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.DTO;

namespace Domain.InterfaceDefinitions.Gateways;

public interface IBaseGateway<TJsonifiable>
{
    public Task<string?> SendMessage(string restCall, TJsonifiable jsonifiable);
}