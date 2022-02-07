using System;

namespace FlyingPizza.Drone
{
    public sealed record Point(double X, double Y) 
    {
        public const double Tolerance = 0.0000001;
        public bool Equals(Point other) =>
            other != null
            && Math.Abs(X - other.X) < Tolerance
            && Math.Abs(Y - other.Y) < Tolerance;
        
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
