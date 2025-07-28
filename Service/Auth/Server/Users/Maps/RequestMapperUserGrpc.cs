using Auth.Server.Users.Model;
using User;

namespace Auth.Server.Users.Maps
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
                Password = response.User.Password,
                Roles = (Model.ROLES)response.User.Roles,
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
                Password = response.Password,
                Roles = (Model.ROLES)response.Roles,
            };

            return user;
        }
    }
}
