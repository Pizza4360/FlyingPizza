using System;
using System.Linq;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id
    {
        get => _id ??= GenerateNewId();
        set => _id = value;
    }
    private string _id;
    
    private static Random _random = new();
    const string chars = "abcdef0123456789";
    private const int IdLength = 24;
    
    // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
    public static string GenerateNewId() => new string(Enumerable.Repeat(chars, IdLength)
                                    .Select(s => s[_random.Next(s.Length)]).ToArray());
}
