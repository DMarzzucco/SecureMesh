using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.JWT.DTOs;
using Auth.Server.Users.Model;

namespace Auth.JWT.Interfaces
{
    public interface IJwtService
    {
        IEnumerable<Claim> GetClaimFromToken();
        Task<string> GenerateEmailVerificationToken(UserModel user);
        Task<string> GenerateRBAToken(UserModel user, string ip, string userAgent, string location);
        Task<string> GenerateRecuperationPasswordToken(UserModel user);
        TokenPair GenerateToken(int sessionId, UserModel user);
        TokenPair RefreshToken(int sessionId, UserModel user);
        bool ValidateToken(string token);
        string GetClaimsValue(IEnumerable<Claim> claims, string type);
        Task<JwtSecurityToken?> ValidateVerificationToken(string token);
        bool IsTokenExpirationSoon(string token);
        TokenPair CreateTokenPair(int  sessionId, UserModel user, DateTime accessTokenExpiration, DateTime refreshTokenExpiration);
    }
}
