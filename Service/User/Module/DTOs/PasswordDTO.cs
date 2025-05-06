using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;

namespace User.Module.DTOs;

/// <summary>
/// Password DTO to verification credentials
/// </summary>
public class PasswordDTO
{
    [SwaggerSchema("User password to validate credentials")]
    [SwaggerSchemaExample("Pr@motheus98")]
    public required string Password { get; set; }
}
