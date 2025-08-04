using User.Module.DTOs;
using User.Module.Model;

namespace User.Module.Service.Interface
{
    public interface IUserService
    {
        Task<UserDTO> GetUserProfileById(int id);
        Task<UserModel> GetUserByEmail(string email);
        Task<UserModel> RegisterUser(CreateUserDTO body);
        Task<UserModel> FindUserById(int id);
        Task<IEnumerable<UserDTO>> ListOfAllRegister();
        Task<UserModel> UpdateEmail(int id, string password, string newEmail);
        Task<UserModel> UpdateRegister( int id, UpdateUserDTO body);
        Task<string> UpdateOwnRegister(int id, UpdateOwnUserDTO body);
        Task<string> UpdatePassword(int id, string oldPassword, string newPassword);
        Task<string> UpdateRoles(int id, ROLES roles);
        Task RemoveUserRegister(int id);
        Task<UserModel> ReturnPasswordAsync(int id, string newPassword);
    }
}
