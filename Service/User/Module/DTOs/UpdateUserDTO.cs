using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;

namespace User.Module.DTOs;

public class UpdateUserDTO
{
    [SwaggerSchema("User name")]
    [SwaggerSchemaExample("Dario Marzzucco")]
    public string? FullName { get; set; }

    [SwaggerSchema("User username")]
    [SwaggerSchemaExample("derkmarzz77")]
    public string? Username { get; set; }
}
