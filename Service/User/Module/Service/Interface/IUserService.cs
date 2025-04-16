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
        Task<string> UpdatePassword (int id, string oldPassword, string password);
        Task RemoveUserRegister(int id);
    }
}
