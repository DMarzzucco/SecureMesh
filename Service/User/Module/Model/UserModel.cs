using Swashbuckle.AspNetCore.Annotations;
using User.Module.Enums;

namespace User.Module.Model;

public class UserModel
{
    [SwaggerSchema("User Id")]
    public int Id { get; set; }

    [SwaggerSchema("User name")]
    public required string complete_name { get; set; }
    
    [SwaggerSchema("User username")]
    public required string username { get; set; }
    
    [SwaggerSchema("User email")]
    public required string email { get; set; }
    
    [SwaggerSchema("User password")]
    public required string password { get; set; }
    
    [SwaggerSchema("User roles")]
    public required ROLES roles { get; set; }

    [SwaggerIgnore]
    public string? RefreshToken { get; set; }
}