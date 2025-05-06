using User.Module.DTOs;
using User.Module.Model;

namespace User.Module.Service.Interface
{
    public interface IUserService
    {
        Task<UserDTO> GetUserProfileById(int id);
        Task<UserModel> RegisterUser(CreateUserDTO body);
        Task<UserModel> FindUserById(int id);
        Task<IEnumerable<UserDTO>> ListOfAllRegister();
        Task<UserModel> UpdateRefreshToken(int id, string RefreshToken);
        Task<UserModel> UpdateRegister(UpdateUserDTO body, int id);
        Task<string> UpdateOwnRegister (int id, UpdateOwnUserDTO body);
        Task<string> UpdatePassword (int id, UpdatePasswordDTO dt);
        Task<string> UpdateRoles(int id, RolesDTO newRoles);
        Task RemoveUserRegister(int id);
        Task<string> RemoveUserRegisterForBasicRoles (int id, PasswordDTO dt);
    }
}
