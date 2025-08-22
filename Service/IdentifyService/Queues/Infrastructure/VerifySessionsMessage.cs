using System;

namespace IdentifyService.Queues.Infrastructure;

public class VerifySessionsMessage
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string UserAgent { get; set; }
    public required string Location { get; set; }
}
