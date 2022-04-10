namespace Domain.DTO
{
    public class BaseDTO
    {
        public override string ToString()
            => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
