using System;
using System.Security.Claims;
using IdentifyService.JWT.DTOs;
using IdentifyService.Server.UMS.Model;
using Microsoft.IdentityModel.Tokens;

namespace IdentifyService.JWT.Helper.Interfaces;

public interface ITokenCreationServices
{
    TokenPair CreateTokenPair(int sessionId, UserModel user, DateTime accessTokenExpiration, DateTime refreshTokenExpiration);
    SecurityTokenDescriptor TokenDescriptionTemplate(UserModel user, string purpose, DateTime expiration);
    string CreateToken(IEnumerable<Claim> claims, SigningCredentials signing, DateTime expiration);
}
