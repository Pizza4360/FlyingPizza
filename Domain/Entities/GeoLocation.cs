using System;

namespace Domain.Entities
{
    public class GeoLocation
    {
        private const decimal Tolerance = 0.0000001m;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public bool Equals(GeoLocation other) =>
            other != null
            && Math.Abs(Latitude - other.Latitude) < Tolerance
            && Math.Abs(Longitude - other.Longitude) < Tolerance;

        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
        public override string ToString()
        {
            return $"GeoLocation:{{x:{Latitude},y:{Longitude}}}";
        }
    }
}
