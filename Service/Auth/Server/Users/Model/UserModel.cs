using SwaggerSchemaExample.Nuget;
using Swashbuckle.AspNetCore.Annotations;

namespace Auth.Server.Users.Model
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
        [SwaggerSchema("2FA Code")]
        [SwaggerSchemaExample("0")]
        public required string Code { get; set; }

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

    public class RemoveOwnAccountDTO
    {
        [SwaggerSchema("2FA Code")]
        [SwaggerSchemaExample("0")]
        public required string Code { get; set; }
        
        [SwaggerSchema("User password")]
        [SwaggerSchemaExample("Pr@motheus98")]
        public required string Password { get; set; }
    }

    public class UpdatePasswordDTO
    {
        [SwaggerSchema("2FA Code")]
        [SwaggerSchemaExample("0")]
        public required string Code { get; set; }

        [SwaggerSchema("Old Password")]
        [SwaggerSchemaExample("Pr@motheus98")]
        public required string OldPassword { get; set; }

        [SwaggerSchema("New Password")]
        [SwaggerSchemaExample("Sr@motheus23")]
        public required string NewPassword { get; set; }
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

        [SwaggerSchema("User password")]
        [SwaggerSchemaExample("Pr@motheus98")]
        public required string Password { get; set; }

        [SwaggerSchema("User roles")]
        [SwaggerSchemaExample("ADMIN")]
        public required ROLES Roles { get; set; }
    }
}
