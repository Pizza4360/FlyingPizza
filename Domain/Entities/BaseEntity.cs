using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace Domain.Entities;

public class BaseEntity
{
    [JsonProperty("Id")]
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    public string Id{get;set;}
    
    private static Random _random = new();
    const string chars = "abcdef0123456789";
    private const int IdLength = 24;
    private static int[] GuidLengths = { 8, 4, 4, 4, 12 };

    public static string GenerateNewGuid()
    => string.Join("-", GuidLengths.Select(IdPart));

    private static string IdPart(int length)
        => new(Enumerable.Repeat(chars, length)
                         .Select(s => s[_random.Next(s.Length)])
                         .ToArray());
    // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
    public static string GenerateNewId() => IdPart(IdLength);
}
