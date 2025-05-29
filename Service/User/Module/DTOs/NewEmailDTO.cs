using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;

namespace User.Module.DTOs;

public class NewEmailDTO
{
    [SwaggerSchema("User password to validate credentials")]
    [SwaggerSchemaExample("Pr@motheus98")]
    public required string Password { get; set; }

    [SwaggerSchema("User email")]
    [SwaggerSchemaExample("marzz77_@gmail.com")]
    public string? NewEmail { get; set; }
}