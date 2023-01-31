using System;
using System.Linq;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public abstract class BaseEntity<TUpdate>
    : IOpeDroneSystemCollectionUpdate<TUpdate>
{
    private const string Chars = "abcdef0123456789";
    private const int IdLength = 24;

    private static readonly Random _random = new();

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public static string GenerateNewId()
    {
        return new string(Enumerable.Repeat(Chars, IdLength)
            .Select(str => str[_random.Next(str.Length)])
            .ToArray());
    }

    public abstract TUpdate Update(); 
}