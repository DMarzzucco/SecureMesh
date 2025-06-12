using Security.Configuration.Swagger;
using Swashbuckle.AspNetCore.Annotations;

namespace Security.Server.Model
{
    public enum ROLES
    {
        ADMIN = 0,
        CREATOR = 1,
        BASIC = 2,
    }
    public class CreateUserDTO
    {
        [SwaggerSchema("User name")]
        [SwaggerSchemaExample("Dario Marzzucco")]
        public required string FullName { get; set; }

        [SwaggerSchema("User username")]
        [SwaggerSchemaExample("derkmarzz77")]
        public required string Username { get; set; }

        [SwaggerSchema("User email")]
        [SwaggerSchemaExample("marzz77_@gmail.com")]
        public required string Email { get; set; }

        [SwaggerSchema("User password")]
        [SwaggerSchemaExample("Pr@motheus98")]
        public required string Password { get; set; }

        [SwaggerSchema("User roles")]
        [SwaggerSchemaExample("ADMIN")]
        public required ROLES Roles { get; set; }
    }
    public class NewEmailDTO
    {
        [SwaggerSchema("User password")]
        [SwaggerSchemaExample("Pr@motheus98")]
        public required string Password { get; set; }

        [SwaggerSchema("User email")]
        [SwaggerSchemaExample("dmarzz_@hotmail.com")]
        public string? NewEmail { get; set; }
    }

    public class PasswordDTO
    {
        [SwaggerSchema("User password")]
        [SwaggerSchemaExample("Sr@motheus23")]
        public required string Password { get; set; }
    }
    public class ForgetPasswordDTO
    {
        [SwaggerSchema("User email")]
        [SwaggerSchemaExample("marzz77_@gmail.com")]
        public required string Email { get; set; }
    }
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
        [SwaggerSchemaExample("Pr@motheus98")]
        public required string Password { get; set; }

        [SwaggerSchema("User roles")]
        [SwaggerSchemaExample("ADMIN")]
        public required ROLES Roles { get; set; }
        
        [SwaggerIgnore]
        public string? CsrfToken { get; set; }

        [SwaggerIgnore]
        public DateTime? CsrfTokenExpiration { get; set; } = null;

        [SwaggerIgnore]
        public string? RefreshToken { get; set; }
    }
}
