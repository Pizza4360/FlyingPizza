using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.DTO.DroneDispatchCommunication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.DTO;

public enum DroneState
{
    Ready,
    Delivering,
    Returning,
    Dead,
    Charging,
    Assigned
}

public enum DroneCommand
{
    DeliverOrder,
    AssignDelivery,
    CompleteDelivery,
    Wait,
    Sleep
}
public static class DroneStateExtensions
{
    private static readonly Dictionary<DroneState, string> StateToColorDict = new(){
        { DroneState.Ready, "#13FF00" },
        { DroneState.Delivering, "#98f5ff" },
        { DroneState.Returning, "#8A2BE2" },
        { DroneState.Dead, "#FF0000" },
        { DroneState.Charging, "#FF9A08" },
        // { DroneState.Assigned, "#FF9A08" }
    };

    public static bool IsValidTransition(DroneState currentState, DroneCommand input)
        => ValidTransitions.ContainsKey(new Tuple<DroneState, DroneCommand>(currentState, input));
    public static readonly Dictionary<Tuple<DroneState, DroneCommand>, DroneState> ValidTransitions = new(){
        {new Tuple<DroneState, DroneCommand>(DroneState.Ready, DroneCommand.AssignDelivery), DroneState.Assigned},
        {new Tuple<DroneState, DroneCommand>(DroneState.Assigned, DroneCommand.DeliverOrder), DroneState.Delivering},
        {new Tuple<DroneState, DroneCommand>(DroneState.Delivering, DroneCommand.CompleteDelivery), DroneState.Returning},
        {new Tuple<DroneState, DroneCommand>(DroneState.Returning, DroneCommand.Wait), DroneState.Ready}
    };

    
    private static readonly Dictionary<DroneState, string> StateToStringDict = new(){
        { DroneState.Ready, "Ready" },
        { DroneState.Delivering, "Delivering" },
        { DroneState.Returning, "Returning" },
        { DroneState.Dead, "Dead" },
        { DroneState.Charging, "Charging" },
        // { DroneState.Assigned, "Assigned" }
    };
    public static string GetColor(this DroneState state) => StateToColorDict[state];
    public static string ToString(this DroneState state) => StateToStringDict[state];
}
