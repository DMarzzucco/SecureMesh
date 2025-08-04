using Auth.JWT.DTOs;
using Auth.JWT.Helper.Interfaces;
using Auth.Server.Users.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.JWT.Helper
{
    public class TokenCreationServices : ITokenCreationServices
    {

        private readonly string _secretKey;

        public TokenCreationServices(IConfiguration configuration)
        {
            var secretKeySection = configuration.GetSection("JwtSettings").GetSection("seecretKey").ToString();

            if (secretKeySection == null || string.IsNullOrEmpty(secretKeySection))
                throw new ArgumentNullException(nameof(secretKeySection), "Secret key cannot be null or empty");

            _secretKey = secretKeySection;
        }
        
        /// <summary>
        /// Template to create authentication token 
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="signing"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public string CreateToken(IEnumerable<Claim> claims, SigningCredentials signing, DateTime expiration)
        {
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = signing
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescription));
        }

        /// <summary>
        /// Template to Create Token Pair
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="user"></param>
        /// <param name="accessTokenExpiration"></param>
        /// <param name="refreshTokenExpiration"></param>
        /// <returns></returns>
        public TokenPair CreateTokenPair(int sessionId, UserModel user, DateTime accessTokenExpiration, DateTime refreshTokenExpiration)
        {
            var keyBytes = Encoding.UTF8.GetBytes(this._secretKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new ("sub", user.Id.ToString()),
                new (ClaimTypes.Role, user.Roles.ToString()),
                new ("sessionId", sessionId.ToString())
            };

            var accessToken = CreateToken(claims, credentials, accessTokenExpiration);
            var refreshToken = CreateToken(claims, credentials, refreshTokenExpiration);


            var refreshHasherToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);

            return new TokenPair
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshHasherToken = refreshHasherToken
            };
        }

        /// <summary>
        /// Template to implement token description
        /// </summary>
        /// <param name="user"></param>
        /// <param name="purpose"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public SecurityTokenDescriptor TokenDescriptionTemplate(UserModel user, string purpose, DateTime expiration)
        {
            var key = Encoding.UTF8.GetBytes(this._secretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim ("sub", user.Id.ToString()),
                    new Claim ("purpose", purpose)
                ]),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            return tokenDescription;
        }
    }
}
