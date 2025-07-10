using System.IdentityModel.Tokens.Jwt;
using Auth.JWT.DTOs;
using Auth.Server.Model;

namespace Auth.JWT.Interfaces
{
    public interface IJwtService
    {
        int GetIdFromToken();
        Task<string> GenerateEmailVerificationToken(UserModel user);
        Task<string> GenerateRecuperationPasswordToken(UserModel user);
        TokenPair GenerateToken(UserModel user);
        TokenPair RefreshToken(UserModel user);
        bool ValidateToken(string token);
        Task <JwtSecurityToken?> ValidateVerificationToken(string token);
        bool IsTokenExpirationSoon(string token);
        TokenPair CreateTokenPair(UserModel user, DateTime accessTokenExpiration, DateTime refreshTokenExpiration);
    }
}
