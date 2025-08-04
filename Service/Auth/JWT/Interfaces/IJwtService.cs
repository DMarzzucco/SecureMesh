using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.JWT.DTOs;
using Auth.Server.Users.Model;

namespace Auth.JWT.Interfaces
{
    public interface IJwtService
    {
        IEnumerable<Claim> GetClaimFromToken();
        Task<string> GenerateEmailVerificationOTT(UserModel user);
        Task<string> GenerateRBAOTT(UserModel user, string ip, string userAgent, string location);
        Task<string> GenerateRecuperationPasswordOTT(UserModel user);
        TokenPair GenerateAuthenticationToken(int sessionId, UserModel user);
        TokenPair GenerateRefreshToken(int sessionId, UserModel user);
        bool ValidateAuthenticationToken(string token);
        string GetValuesFromClaim(IEnumerable<Claim> claims, string type);
        Task<JwtSecurityToken?> ValidateOTT(string token);
        bool IsTokenExpirationSoon(string token);
    }
}
