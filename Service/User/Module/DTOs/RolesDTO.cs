using System;
using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;
using User.Module.Enums;

namespace User.Module.DTOs;

public class RolesDTO
{
    [SwaggerSchema("User Roles")]
    [SwaggerSchemaExample("BASIC")]
    public required ROLES Roles {get;set;}
}
