using Security.Module.DTOs;
using Security.Server.Model;

namespace Security.Module.Services.Interfaces
{
    public interface ISecurityService
    {
        Task<string> ChangeAddressEmail(int id, NewEmailDTO body);
        Task<string> RegisteredUser(CreateUserDTO body);
        Task<string> GenerateToken(UserModel body);
        Task<string> GenerateRefreshToken();
        Task<object> GetProfile();
        Task<UserModel> GetUserByCookie();
        Task<string> ForgetPassword(ForgetPasswordDTO dto);
        Task LogOut();
        Task<string> RemoveOwnAccount(int id, PasswordDTO dto);
        Task<UserModel> RefreshTokenValidate(string refreshToken, int id);
        Task<string> ResetPassword(string token, PasswordDTO body);
        Task<UserModel> ValidateUser(LoginDTO body);
        Task<string> VerificationEmail(string token);
        Task<string> VerificationNewEmail(string token);
    }
}
