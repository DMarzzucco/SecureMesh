using Auth.JWT.DTOs;
using Auth.Server.Users.Model;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Auth.JWT.Helper.Interfaces
{
    public interface ITokenCreationServices
    {
        TokenPair CreateTokenPair(int sessionId, UserModel user, DateTime accessTokenExpiration, DateTime refreshTokenExpiration);
        SecurityTokenDescriptor TokenDescriptionTemplate(UserModel user, string purpose, DateTime expiration);
        string CreateToken(IEnumerable<Claim> claims, SigningCredentials signing, DateTime expiration);

    }
}
