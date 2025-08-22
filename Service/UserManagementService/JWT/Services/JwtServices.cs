using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserManagementService.Configuration.Redis.Repository.Interfaces;
using UserManagementService.JWT.Services.Interfaces;
using UserManagementService.Server.Users.Model;
using UserManagementService.Utils.Exceptions;

namespace UserManagementService.JWT.Services;

public class JwtServices : IJwtServices
{
    private readonly string _secretKey;
    private readonly IRedisRepository _redisRepository;

    public JwtServices(IConfiguration configuration, IRedisRepository redisRepository)
    {
        var secretKeySection = configuration.GetSection("JwtSettings").GetSection("seecretKey").ToString();

        if (secretKeySection == null || string.IsNullOrEmpty(secretKeySection))
            throw new ArgumentNullException(nameof(secretKeySection), "Secret key cannot be null or empty");

        _secretKey = secretKeySection;
        this._redisRepository = redisRepository;
    }


    /// <summary>
    /// Generate Recuperation Password Token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<string> GenerateRecuperationPasswordOTT(UserModel user)
    {
        var key = Encoding.UTF8.GetBytes(this._secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, user.Email),
                new Claim ("sub", user.Id.ToString()),
                new Claim ("purpose", "password_recuperation")
            ]),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescription);
        var ott = tokenHandler.WriteToken(token);

        await this._redisRepository.SetAsync(ott);
        /// save token in redis
        return ott;
    }

    /// <summary>
    /// Validate OTT 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestExceptions"></exception>
    /// <exception cref="SecurityTokenSignatureKeyNotFoundException"></exception>
    /// <exception cref="SecurityTokenExpiredException"></exception>
    public async Task<JwtSecurityToken?> ValidateOTT(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new BadRequestExceptions("Token is required");

        var tk = await this._redisRepository.GetByTokenAsync(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.UTF8.GetBytes(this._secretKey);

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken validateToken)
            throw new SecurityTokenSignatureKeyNotFoundException("Token is invalid");

        if (validateToken.ValidTo < DateTime.UtcNow)
            throw new SecurityTokenExpiredException("Token is expired");

        var verification = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };
        tokenHandler.ValidateToken(tk, verification, out _);

        var jwtToken = tokenHandler.ReadToken(tk) as JwtSecurityToken;
        return jwtToken;
    }

    /// <summary>
    /// Get Claims Values
    /// </summary>
    /// <param name="claims"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public string GetValuesFromClaims(IEnumerable<Claim> claims, string type)
    {
        var value = claims.FirstOrDefault(c => c.Type == type)?.Value;

        if (string.IsNullOrEmpty(value))
            throw new UnauthorizedAccessException($"Invalid tokne, missing claim: {type}");

        return value;
    }
}
