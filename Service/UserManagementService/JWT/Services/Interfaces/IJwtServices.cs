using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserManagementService.Server.Users.Model;

namespace UserManagementService.JWT.Services.Interfaces;

public interface IJwtServices
{
    Task<string> GenerateRecuperationPasswordOTT(UserModel user);
    Task<JwtSecurityToken?> ValidateOTT(string token);
    string GetValuesFromClaims(IEnumerable<Claim> claims, string type);
}
