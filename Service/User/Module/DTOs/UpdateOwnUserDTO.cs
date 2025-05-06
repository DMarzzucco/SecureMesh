using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;

namespace User.Module.DTOs;

/// <summary>
/// Update Own UserDTO
/// </summary>
public class UpdateOwnUserDTO
{
    // password to validate
    [SwaggerSchema("User password")]
    [SwaggerSchemaExample("Pr@motheus98")]
    public required string Password { get; set; }

    //body
    [SwaggerSchema("User name")]
    [SwaggerSchemaExample("Dario Marzzucco")]
    public string? FullName { get; set; }

    [SwaggerSchema("User username")]
    [SwaggerSchemaExample("derkmarzz77")]
    public string? Username { get; set; }

    [SwaggerSchema("User email")]
    [SwaggerSchemaExample("marzz77_@gmail.com")]
    public string? Email { get; set; }
}
