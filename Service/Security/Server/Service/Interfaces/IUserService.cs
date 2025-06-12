using Security.Server.Model;

namespace Security.Server.Service.Interfaces
{
    public interface IUserService
    {
        Task CancelationOperation(int id);
        Task<string> DeletedOwnAccount(int id, PasswordDTO dto);
        Task<UserModel> RegisterUser(CreateUserDTO body);
        Task<UserModel> GetUserById(int id);
        Task<UserModel> GetUserByEmail(string email);
        Task<UserModel> UpdateEmailAdress(int id, NewEmailDTO body);
        Task<UserModel> MarkEmailAsync (int id );
        Task UpdateRefreshToken(int id, string? refreshToken);
        Task<UserModel> ReturnPassword(int id, PasswordDTO body);
        Task UpdateCsrfToken(int id, string csrfToken, DateTime csrfTokenExpiration);
        Task<UserModel> FindByValue(string key, object value);
    }
}
