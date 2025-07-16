
namespace Auth.Queues.Infrastructure;

public class TowAFMessage
{
    public required string Email { get; set; }
    public required string TwoAFCode { get; set; }
}
