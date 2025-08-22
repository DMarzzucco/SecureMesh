using User.Module.DTOs;
using User.Module.Model;

namespace User.Module.Service.Interface
{
    public interface IUserService
    {
        Task<UserModel> RegisterUser(CreateUserDTO body);
        Task<UserModel> VerifyNewEmailParameters(int id, string password, string newEmail);
        Task<string> UpdateOwnRegister(int id, UpdateOwnUserDTO body);
        Task<string> UpdatePassword(int id, string oldPassword, string newPassword);
        Task<UserModel> UpdateEmail(int id, string newEmail);
        Task<string> UpdateRoles(int id, ROLES roles);
        Task<string> RemoveUserRegister(int id);
        Task<UserModel> FindValueByKey(string key, object value);
        Task<UserModel> ReturnPasswordAsync(int id, string newPassword);
    }
}
