using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    /*
    public interface IBaseEntity
    {
        
    }
*/
    
        public interface IBaseEntity : IJsonString
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            
        }
}
