using IdentifyService.Server.UMS.Model;

namespace IdentifyService.Module.DTOs
{
    public class AuthorizationTokenDTO
    {
        public required UserModel User { get; set; }
        public int SessionId { get; set; }
    }
}
