using System;

namespace Security.Module.Model;

public class SessionModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Ip { get; set; }
    public required string UserAgent { get; set; }
    public required string Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }

}
