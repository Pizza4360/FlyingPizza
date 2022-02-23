using System;

namespace FlyingPizza.Drone
{
    public sealed record Point(double Lat, double Long) 
    {
        public const double Tolerance = 0.0000001;
        public bool Equals(Point other) =>
            other != null
            && Math.Abs(Lat - other.Lat) < Tolerance
            && Math.Abs(Long - other.Long) < Tolerance;
        
        public override int GetHashCode() => HashCode.Combine(Lat, Long);
    }
}
