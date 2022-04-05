﻿using System;

namespace FrontEnd
{
    public class GeoLocation
    {
        private const decimal Tolerance = 0.0000001m;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public override bool Equals(object? o)
        {
            return o is GeoLocation other &&
                   Math.Abs(Latitude - other.Latitude) < Tolerance
                   && Math.Abs(Longitude - other.Longitude) < Tolerance;
        }

        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
        public override string ToString()
        {
            return $"GeoLocation:{{x:{Latitude},y:{Longitude}}}";
        }
    }
}
