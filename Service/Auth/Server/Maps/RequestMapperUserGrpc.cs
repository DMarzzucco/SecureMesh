using Auth.Server.Model;
using User;

namespace Auth.Server.Maps
{
    public class RequestMapperUserGrpc
    {
        public UserModel InvokeValidationResponseMap(ValidationResponse response)
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
                TwoAFCode = response.User.TwoAfCode,
                TwoAFCodeExpiration = response.User.TwoAfCodeExpiration != null
                        ? response.User.TwoAfCodeExpiration.ToDateTime()
                        : DateTime.MinValue,
                RefreshToken = response.User.RefreshToken
            };
            return user;
        }

        public UserModel InvokeUserModel(AuthUserResponse response)
        {
            var user = new UserModel {
                Id = response.Id,
                FullName = response.FullName,
                Username = response.Username,
                Email = response.Email,
                EmailVerified = response.EmailVerified,
                Password = response.Password,
                Roles = (Model.ROLES)response.Roles,
                TwoAFCode = response.TwoAfCode,
                TwoAFCodeExpiration = response.TwoAfCodeExpiration != null
                        ? response.TwoAfCodeExpiration.ToDateTime()
                        : DateTime.MinValue,
                RefreshToken = response.RefreshToken
            };

            return user;
        }
    }
}
