﻿using Security.Configuration.Swagger;
using Swashbuckle.AspNetCore.Annotations;
using Security.Server.Model;

namespace Security.Server.DTOs
{
    public class UserDTO
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

        [SwaggerSchema("User roles")]
        [SwaggerSchemaExample("ADMIN")]
        public required ROLES Roles { get; set; }
    }
}
