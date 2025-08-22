using IdentifyService.Module.DTOs;
using IdentifyService.Server.UMS.Model;
using IdentifyService.Server.UMS.DTOs;
using IdentifyService.Server.UMS.Model;

namespace IdentifyService.Module.Services.Interfaces
{
    public interface IIdentityProviderService
    {
        Task<string> Login(UserModel body);
        Task<string> VerifySession(string token);
        Task<string> InitSession(VerifyCodeDTO dto);
        Task<string> ChangePassword(UpdatePasswordDTO body);
        Task<string> ChangeAddressEmail(NewEmailDTO body);
        Task<string> RegisteredUser(CreateUserDTO body);
        Task<string> GenerateRefreshToken();
        Task<IEnumerable<SessionModel?>> ListOfAllSessionsAsync();
        Task<string> RemoveOneSessionById(int id);
        Task<AuthorizationTokenDTO> GetValueByCookie();
        Task LogOut();
        Task<string> RemoveOwnAccount(RemoveOwnAccountDTO dto);
        Task<string> TwoFactorAuthenticationCodeGeneration();
        Task RefreshTokenValidate(string refreshToken, int id);
        Task<UserModel> ValidateUserCredential(LoginDTO body);
        Task<string> VerificationEmail(string token);
    }
}
