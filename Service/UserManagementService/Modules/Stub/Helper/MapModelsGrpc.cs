using System;
using UserManagementService.Proto.Server;
using UserManagementService.Server.Users.Model;

namespace UserManagementService.Modules.Stub.Helper;

public class MapModelsGrpc
{
    public UserResponse UserModelMapper(UserModel user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            Roles = (Proto.Server.ROLES)user.Roles
        };
    }

    public CreateUserDTO CreateUserMapper(CreateUserRequest dto)
    {
        return new CreateUserDTO
        {
            FullName = dto.FullName,
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            Roles = (Server.Users.Model.ROLES)dto.Roles
        };
    }
}
