using System;
using UserManagementService.Server.Users.Model;

namespace UserManagementService.Server.Users.Service.Interfaces;

public interface IUserService
{
    Task RemoveUser(int id);
    Task<UserModel> RegisterUser(CreateUserDTO body);
    Task<IEnumerable<UserDTO>> ListOfAllUsers();

    Task<string> RemoveAnyAccount(int id);
    Task<UserModel> GetUserById(int id);
    Task<string> UpdateOwnRegister(int id, UpdateOwnRegisterDTO body);
    Task<string> UpdateUserRoles(int id, RolesDTO body);
    Task<UserDTO> GetUserProfile(int id);
    Task<UserModel> GetUserByEmail(string email);
    Task<string> UpdatePasswordUser(int id, UpdatePasswordDTO body);
    Task<UserModel> VerifyNewEmailAdressParameters(int id, NewEmailDTO body);
    Task<UserModel> UpdateEmailAddress(int id, string newEmail);
    Task<UserModel> ReturnPassword(int id, PasswordDTO body);
    Task<UserModel> FindByValue(string key, object value);
}
