using System.Runtime.Serialization;

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
