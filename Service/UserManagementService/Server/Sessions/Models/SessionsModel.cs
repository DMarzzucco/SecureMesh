using System;

namespace UserManagementService.Server.Sessions.Models;

public class SessionsModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Ip { get; set; }
    public required string UserAgent { get; set; }
    public required string Location { get; set; }
    public bool IsActive { get; set; }
}
