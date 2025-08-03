using System;
using SwaggerSchemaExample.Nuget;
using Swashbuckle.AspNetCore.Annotations;
using User.Module.Enums;

namespace User.Module.DTOs;

public class RolesDTO
{
    [SwaggerSchema("User Roles")]
    [SwaggerSchemaExample("BASIC")]
    public required ROLES Roles {get;set;}
}
