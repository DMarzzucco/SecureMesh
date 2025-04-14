using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;
using User.Module.Enums;

namespace User.Module.DTOs;

public class CreateUserDTO
{
    [SwaggerSchema("User name")]
    [SwaggerSchemaExample("Dario Marzzucco")]
    public required string FullName { get; set; }

    [SwaggerSchema("User username")]
    [SwaggerSchemaExample("derkmarzz77")]
    public required string Username { get; set; }

    [SwaggerSchema("User email")]
    [SwaggerSchemaExample("marzz77_@gmail.com")]
    public required string Email { get; set; }

    [SwaggerSchema("User password")]
    [SwaggerSchemaExample("Pr@motheus98")]
    public required string Password { get; set; }

    [SwaggerSchema("User roles")]
    [SwaggerSchemaExample("ADMIN")]
    public required ROLES Roles { get; set; }
}