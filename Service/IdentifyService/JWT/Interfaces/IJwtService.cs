using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentifyService.JWT.DTOs;
using IdentifyService.Server.UMS.Model;

namespace IdentifyService.JWT.Interfaces
{
    public interface IJwtService
    {
        IEnumerable<Claim> GetClaimFromToken();
        Task<string> GenerateEmailVerificationOTT(UserModel user);
        Task<string> GenerateRBAOTT(UserModel user, string ip, string userAgent, string location);
        Task<string> GenerateVerifyNewEmailOTT(UserModel user, int sessionId, string newEmail);
        Task<string> GenerateRecuperationPasswordOTT(UserModel user);
        TokenPair GenerateAuthenticationToken(int sessionId, UserModel user);
        TokenPair GenerateRefreshToken(int sessionId, UserModel user);
        bool ValidateAuthenticationToken(string token);
        string GetValuesFromClaims(IEnumerable<Claim> claims, string type);
        Task<JwtSecurityToken?> ValidateOTT(string token);
        bool IsTokenExpirationSoon(string token);
    }
}
