namespace User.Module.Model;

public class UserModel
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required ROLES Roles { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}