using System;
using System.Linq;
using System.Runtime.Serialization;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO;

public class BaseDto : IBaseEntity
{
    public string ToJsonString()
        => Newtonsoft.Json.JsonConvert.SerializeObject(this);

    private string _id { get; set; }

    [BsonId]
    [BsonElement("Id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id
    {
        get => _id ??= GenerateNewId();
        set => _id ??= value;
    }
    
    private static Random _random = new();
    const string chars = "abcdef0123456789";
    private const int IdLength = 24;
    
    // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
    private static string GenerateNewId()
    {
        return new string(Enumerable.Repeat(chars, IdLength)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}