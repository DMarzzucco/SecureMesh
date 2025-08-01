using Auth.Module.DTOs;
using Auth.Server.Security.Model;
using Auth.Server.Users.DTOs;
using Auth.Server.Users.Model;

namespace Auth.Module.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> Login(UserModel body);
        Task<string> VerifySession(string token);
        Task<string> InitSession(VerifyCodeDTO dto);
        Task<string> ChangePassword(UpdatePasswordDTO body);
        Task<string> ChangeAddressEmail( NewEmailDTO body);
        Task<string> RegisteredUser(CreateUserDTO body);
        Task<string> GenerateRefreshToken();
        Task<IEnumerable<SessionModel?>> ListOfAllSessionsAsync();
        Task<string> RemoveOneSessionById(int id);
        Task<AuthorizationTokenDTO> GetValueByCookie();
        Task<string> ForgetPassword(ForgetPasswordDTO dto);
        Task LogOut();
        Task<string> RemoveOwnAccount( RemoveOwnAccountDTO dto);
        Task<string> TwoFactorAuthenticationCodeGeneration();
        Task RefreshTokenValidate(string refreshToken, int id);
        Task<string> ResetPassword(string token, PasswordDTO body);
        Task<UserModel> ValidateUserCredentials(LoginDTO body);
        Task<string> VerificationEmail(string token);
        Task<string> VerificationNewEmail(string token);
    }
}
