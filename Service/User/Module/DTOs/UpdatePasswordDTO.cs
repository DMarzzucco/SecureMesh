using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;

namespace User.Module.DTOs;

/// <summary>
/// Update password DTO
/// </summary>
public class UpdatePasswordDTO
{
    [SwaggerSchema("Old Password")]
    [SwaggerSchemaExample("Pr@motheus98")]
    public required string OldPassword { get; set; }

    [SwaggerSchema("New Password")]
    [SwaggerSchemaExample("Sr@motheus23")]
    public required string NewPassword { get; set; }

}
