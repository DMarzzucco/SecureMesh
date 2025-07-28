using Auth.Server.Users.Model;

namespace Auth.Server.Users.Service.Interfaces
{
    public interface IUserService
    {
        Task RemoveUser(int id);
        Task<UserModel> RegisterUser(CreateUserDTO body);
        Task<UserModel> GetUserById(int id);
        Task<UserModel> GetUserByEmail(string email);
        Task<string> UpdatePasswordUser(int id, UpdatePasswordDTO body);
        Task<UserModel> UpdateEmailAddress(int id, NewEmailDTO body);
        Task<UserModel> ReturnPassword(int id, PasswordDTO body);
        Task<UserModel> FindByValue(string key, object value);
    }
}
