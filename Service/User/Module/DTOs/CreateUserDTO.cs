using User.Module.Enums;

namespace User.Module.DTOs;

public class CreateUserDTO
{
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required ROLES Roles { get; set; }
}