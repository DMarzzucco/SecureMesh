using Security.Server.Model;
using User;

namespace Security.Server.Maps
{
    public class RequestMapperUserGrpc
    {
        public UserModel InvokeMap(ValidationResponse response)
        {
            var user = new UserModel
            {
                Id = response.User.Id,
                FullName = response.User.FullName,
                Username = response.User.Username,
                Email = response.User.Email,
                EmailVerified = response.User.EmailVerified,
                Password = response.User.Password,
                Roles = (Model.ROLES)response.User.Roles,
                CsrfToken = response.User.CsrfToken,
                CsrfTokenExpiration = response.User.CsrfTokenExpiration != null
                        ? response.User.CsrfTokenExpiration.ToDateTime()
                        : DateTime.MinValue,
                RefreshToken = response.User.RefreshToken
            };
            return user;
        }
    }
}
