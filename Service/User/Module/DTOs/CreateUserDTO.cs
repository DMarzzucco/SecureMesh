using Swashbuckle.AspNetCore.Annotations;
using User.Module.Enums;

namespace User.Module.DTOs;

public class CreateUserDTO
{
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
}