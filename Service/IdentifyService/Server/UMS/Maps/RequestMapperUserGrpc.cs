using IdentifyService.Server.UMS.Model;
using UserManagementService.Proto.Server;

namespace IdentifyService.Server.UMS.Maps
{
    public class RequestMapperUserGrpc
    {
        public CreateUserRequest InvokeCreateUser(CreateUserDTO dto)
        {
            return new CreateUserRequest
            {
                FullName = dto.FullName,
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                Roles = (UserManagementService.Proto.Server.ROLES)dto.Roles
            };
        }

        public UserModel InvokeValidationResponseMap(MultipleUserResponse response)
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

        public UserModel InvokeUserModel(UserResponse response)
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
