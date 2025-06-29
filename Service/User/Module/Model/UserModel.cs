﻿using Swashbuckle.AspNetCore.Annotations;
using User.Configuration.Swagger.Attributes;

namespace User.Module.Model;

public class UserModel
{
    [SwaggerSchema("User Id")]
    public int Id { get; set; }

    [SwaggerSchema("User name")]
    [SwaggerSchemaExample("Dario Marzzucco")]
    public required string FullName { get; set; }

    [SwaggerSchema("User username")]
    [SwaggerSchemaExample("derkmarzz77")]
    public required string Username { get; set; }

    [SwaggerSchema("User email")]
    [SwaggerSchemaExample("marzz77_@gmail.com")]
    public required string Email { get; set; }

    [SwaggerIgnore]
    public bool EmailVerified { get; set; } = false;

    [SwaggerSchema("User password")]
    [SwaggerSchemaExample("passmort243")]
    public required string Password { get; set; }

    [SwaggerSchema("User roles")]
    [SwaggerSchemaExample("ADMIN")]
    public required ROLES Roles { get; set; }

    [SwaggerIgnore]
    public bool IsDeleted { get; set; } = false;

    [SwaggerIgnore]
    public DateTime? DeletedAt { get; set; } = null;

    [SwaggerIgnore]
    public string? ScheduledDeletionJobId { get; set; }

    [SwaggerIgnore]
    public string? CsrfToken { get; set; }

    [SwaggerIgnore]
    public DateTime? CsrfTokenExpiration { get; set; } = null;

    [SwaggerIgnore]
    public string? RefreshToken { get; set; }
}