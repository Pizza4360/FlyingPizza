using System.Net.Http;
using System.Threading.Tasks;

namespace Domain.DTO;

public class OkToCompletePost : BaseDTO
{
    public Task<HttpContent> Message { get; set; }
    public bool DoComplete { get; set; }
}