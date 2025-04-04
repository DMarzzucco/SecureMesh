using Security.Module.DTOs;
using Security.Server.Model;

namespace Security.Module.Services.Interfaces
{
    public interface ISecurityService
    {
        Task<string> GenerateToken(UserModel body);
        Task<string> GenerateRefreshToken();
        Task<object> GetProfile();
        Task<UserModel> GetUserByCookie();
        Task LogOut();
        Task<UserModel> RefreshTokenValidate(string refreshToken, int id);
        Task<UserModel> ValidateUser(LoginDTO body);
    }
}
