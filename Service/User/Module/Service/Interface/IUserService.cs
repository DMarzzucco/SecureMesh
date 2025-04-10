using User.Module.DTOs;
using User.Module.Model;

namespace User.Module.Service.Interface
{
    public interface IUserService
    {
        Task<UserDTO> GetUserProfileById(int id);
        Task<UserModel> RegisterUser(CreateUserDTO body);
        Task<UserModel> FindUserById(int id);
        Task<IEnumerable<UserModel>> ListOfAllRegister();
        Task<UserModel> UpdateRefreshToken(int id, string RefreshToken);
        Task<UserModel> UpdateRegister(UpdateUserDTO body, int id);
        Task RemoveUserRegister(int id);
    }
}
