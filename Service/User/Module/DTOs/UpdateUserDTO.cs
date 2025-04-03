using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;
using User.Module.Enums;

namespace User.Module.DTOs;

public class UpdateUserDTO
{
    [SwaggerSchema("User name")]
    [SwaggerSchemaExample("Dario Marzzucco")]
    public string? FullName { get; set; }

    [SwaggerSchema("User username")]
    [SwaggerSchemaExample("derkmarzz77")]
    public string? Username { get; set; }

    [SwaggerSchema("User email")]
    [SwaggerSchemaExample("marzz77_@gmail.com")]
    public string? Email { get; set; }

    [SwaggerSchema("User password")]
    [SwaggerSchemaExample("passmort243")]
    public string? Password { get; set; }

    [SwaggerSchema("User roles")]
    [SwaggerSchemaExample("ADMIN")]
    public ROLES? Roles { get; set; }
}