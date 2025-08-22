namespace User.Module.DTOs;

/// <summary>
/// Update Own UserDTO
/// </summary>
public class UpdateOwnUserDTO
{
    public required string Password { get; set; }
    public string? FullName { get; set; }
    public string? Username { get; set; }
}
