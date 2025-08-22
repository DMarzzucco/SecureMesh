using System;

namespace UserManagementService.Modules.Models;

public class ManagementUserModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;
    public string? ScheduledDeletionJobId { get; set; }
    public bool IsDisabled { get; set; } = false;
    public int Attemps { get; set; } = 0;
    public DateTime? LockedAt { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
