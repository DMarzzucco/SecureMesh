using Google.Protobuf.WellKnownTypes;
using User.Module.Model;
namespace User.Module.Stubs.Maps
{
    public class MapResponseGrpc()
    {
        public AuthUserResponse InvokeMap(UserModel user)
        {
            return new AuthUserResponse
            {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    EmailVerified = user.EmailVerified,
                    Password = user.Password,
                    Roles = user.Roles,
                    CsrfToken = user.CsrfToken ?? "",
                    CsrfTokenExpiration = user.CsrfTokenExpiration.HasValue
                        ? Timestamp.FromDateTime(user.CsrfTokenExpiration.Value.ToUniversalTime())
                        : null,
                    RefreshToken = user.RefreshToken ?? ""
            };
        }
    }
}
