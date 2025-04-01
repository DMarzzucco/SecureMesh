using Swashbuckle.AspNetCore.Annotations;
using User.Module.Enums;

namespace User.Module.DTOs;

public class UpdateUserDTO
{
    [SwaggerSchema("User name")]
    public string? complete_name { get; set; }
    
    [SwaggerSchema("User username")]
    public string? username { get; set; }
    
    [SwaggerSchema("User email")]
    public string? email { get; set; }
    
    [SwaggerSchema("User password")]
    public string? password { get; set; }
    
    [SwaggerSchema("User roles")]
    public ROLES? roles { get; set; }
}