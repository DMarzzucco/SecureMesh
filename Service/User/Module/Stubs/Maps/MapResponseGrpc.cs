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
                    Password = user.Password,
                    Roles = user.Roles
            };
        }
    }
}
