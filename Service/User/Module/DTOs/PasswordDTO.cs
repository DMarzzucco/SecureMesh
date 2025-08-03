using SwaggerSchemaExample.Nuget;
using Swashbuckle.AspNetCore.Annotations;

namespace User.Module.DTOs;

/// <summary>
/// Password DTO to verification credentials
/// </summary>
public class PasswordDTO
{
    [SwaggerSchema("User password to validate credentials")]
    [SwaggerSchemaExample("Sr@motheus23")]
    public required string Password { get; set; }
}
