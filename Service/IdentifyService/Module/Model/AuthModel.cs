using System;

namespace IdentifyService.Module.Model;

public class AuthModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool EmailVerify { get; set; } = false;
    public string? TwoFACode { get; set; }
    public DateTime? TwoFACodeExpiration { get; set; } = null;
    public int VerifyAttempts { get; set; } = 0;
    public DateTime? LockedAt { get; set; } = null;
    public string? RefreshToken { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}
