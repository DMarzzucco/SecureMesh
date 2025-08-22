using IdentifyService.Configuration.Swagger;
using Swashbuckle.AspNetCore.Annotations;
using IdentifyService.Server.UMS.Model;

namespace IdentifyService.Server.UMS.DTOs
{
    public class VerifyCodeDTO
    {
        [SwaggerSchema("User email")]
        [SwaggerSchemaExample("marzz77_@gmail.com")]
        public required string Email { get; set; }  
        
        [SwaggerSchema("Code 2FA")]
        [SwaggerSchemaExample("0")]
        public required string TwoAfCode { get; set; }
    }
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
