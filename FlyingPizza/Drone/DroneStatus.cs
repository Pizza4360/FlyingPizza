using System;

namespace FlyingPizza.Drone
{
    public enum DroneState
    {
        Ready,
        Delivering,
        Returning,
        Dead,
        Charging
    }
    
    public record DroneStatus(DroneState State)
    {
       
        public override string ToString()
        {
            
            switch (State)
            {
                case DroneState.Ready:
                    return "Ready";
                case DroneState.Charging:
                    return "Charging";
                case DroneState.Dead:
                    return "Dead";
                case DroneState.Delivering:
                    return "Delivering";
                case DroneState.Returning:
                    return "Returning";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
