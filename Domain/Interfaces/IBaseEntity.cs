using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.Interfaces
{
    public class BaseEntity
    {
        private static Random _random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int IdLength = 24;
        
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string? Id { get; set; }
        // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
        public static string GenerateNewID()
        {
            return new string(Enumerable.Repeat(chars, IdLength)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
