using System.Collections.Generic;

namespace Domain.DTO;

public static class DroneStateExtensions
{
    private static readonly Dictionary<DroneState, string> StateToColorDict = new(){
        { DroneState.Ready, "#13FF00" },
        { DroneState.Delivering, "#98f5ff" },
        { DroneState.Returning, "#8A2BE2" },
        { DroneState.Dead, "#FF0000" },
        { DroneState.Charging, "#FF9A08" }
    };
    private static readonly Dictionary<DroneState, string> StateToStringDict = new(){
        { DroneState.Ready, "Ready" },
        { DroneState.Delivering, "Delivering" },
        { DroneState.Returning, "Returning" },
        { DroneState.Dead, "Dead" },
        { DroneState.Charging, "Charging" }
    };
    public static string GetColor(this DroneState state) => StateToColorDict[state];
    public static string ToString(this DroneState state) => StateToStringDict[state];
}