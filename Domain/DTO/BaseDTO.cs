using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Domain.DTO
{
    public class BaseDTO : IJsonString
    {
        public string ToJsonString()
            => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }

    public interface IJsonString
    {
        public string ToJsonString();
    }
}
