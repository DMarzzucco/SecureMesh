using Security.JWT.DTOs;
using Security.Server.Model;

namespace Security.JWT.Interfaces
{
    public interface IJwtService
    {
        int GetIdFromToken();
        TokenPair GenerateToken(UserModel user);
        TokenPair RefreshToken(UserModel user);
        bool ValidateToken(string token);
        bool isTokenExpirationSoon(string token);
        TokenPair CreateTokenPair(UserModel user, DateTime accessTokenExpiration, DateTime refreshTokenExpiration);
    }
}
