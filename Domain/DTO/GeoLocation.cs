using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO
{
    public class GeoLocation : BaseDto
    {
        private const decimal Tolerance = 0.0000001m;

        [BsonElement("Latitude")]
        [JsonPropertyName("Latitude")]
        [BsonRepresentation(BsonType.String)]
        public decimal Latitude { get; set; }

        [BsonElement("Longitude")]
        [JsonPropertyName("Longitude")]
        [BsonRepresentation(BsonType.String)]
        public decimal Longitude { get; set; }

        public override bool Equals(object? o)
        {
            return o is GeoLocation other &&
                   Math.Abs(Latitude - other.Latitude) < Tolerance
                   && Math.Abs(Longitude - other.Longitude) < Tolerance;
        }

        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
    }
}
