using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Domain.Interfaces;
using MongoDB.Bson;

namespace Domain.Interfaces
{
    /*
    public interface IBaseEntity
    {
        
    }
*/
    
        public interface IBaseEntity
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
        }
}
