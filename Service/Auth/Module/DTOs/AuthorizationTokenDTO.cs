using Auth.Server.Users.Model;

namespace Auth.Module.DTOs
{
    public class AuthorizationTokenDTO
    {
        public required UserModel User { get; set; }
        public int SessionId { get; set; }
    }
}
