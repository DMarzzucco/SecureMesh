using Security.Configuration.Swagger;
using Swashbuckle.AspNetCore.Annotations;

namespace Security.Module.DTOs
{
    public class LoginDTO
    {
        [SwaggerSchema("User username")]
        [SwaggerSchemaExample("derkmarzz77")]
        public required string Username { get; set; }

        [SwaggerSchema("User password")]
        [SwaggerSchemaExample("passmort243")]
        public required string Password { get; set; }
    }
}
