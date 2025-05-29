namespace Security.Queues.Infrastructure;

/// <summary>
/// Email Verification Message Structure
/// </summary>
public class EmailVerifcationMessage
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public int Id { get; set; }
}
