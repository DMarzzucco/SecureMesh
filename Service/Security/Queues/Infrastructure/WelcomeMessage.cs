namespace Security.Queues.Infrastructure;

/// <summary>
/// Welcome Message Structure 
/// </summary>
public class WelcomeMessage
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public int Id { get; set; }
}
