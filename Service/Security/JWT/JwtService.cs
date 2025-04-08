using Microsoft.IdentityModel.Tokens;
using Security.JWT.DTOs;
using Security.JWT.Interfaces;
using Security.Server.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Security.JWT
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly IHttpContextAccessor _context;

        public JwtService(IConfiguration configuration, IHttpContextAccessor context)
        {
            var secretKeySection = configuration.GetSection("JwtSettings").GetSection("seecretKey").ToString();

            if (secretKeySection == null || string.IsNullOrEmpty(secretKeySection))
                throw new ArgumentNullException(nameof(secretKeySection), "Secret key cannot be null or empty");

            _secretKey = secretKeySection;
            _context = context;
        }

        /// <summary>
        /// Get Id From Token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int GetIdFromToken()
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("Http Context is null");

            var token = httpContext.Request.Cookies["Authentication"] ??
                throw new UnauthorizedAccessException("Token not found");

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var idClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                throw new UnauthorizedAccessException("Invalid Token");

            return int.Parse(idClaim);
        }
        /// <summary>
        /// Generate Token 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TokenPair GenerateToken(UserModel user)
        {
            return CreateTokenPair(
                user,
                DateTime.UtcNow.AddHours(5),
                DateTime.UtcNow.AddDays(5)
                );
        }
        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TokenPair RefreshToken(UserModel user)
        {
            return CreateTokenPair(
                user,
                DateTime.UtcNow.AddDays(5),
                DateTime.UtcNow.AddDays(5)
                );
        }
        /// <summary>
        /// Validate if token expire soon 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool isTokenExpirationSoon(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token)) return false;

            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null) return false;

            var expiration = jwtToken.ValidTo;

            return expiration <= DateTime.UtcNow.AddMinutes(60);
        }
        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(this._secretKey);

            var principal = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                tokenHandler.ValidateToken(token, principal, out _);
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                return false;
            }

            throw new NotImplementedException();

        }
        /// <summary>
        /// Create Token Pair template
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accessTokenExpiration"></param>
        /// <param name="refreshTokenExpiration"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TokenPair CreateTokenPair(UserModel user, DateTime accessTokenExpiration, DateTime refreshTokenExpiration)
        {
            var keyBytes = Encoding.UTF8.GetBytes(this._secretKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new ("sub", user.Id.ToString()),
                new (ClaimTypes.Role, user.Roles.ToString()),
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
        /// Template to create token
        /// </summary>
        /// <returns></returns>
        private static string CreateToken(IEnumerable<Claim> claims, SigningCredentials signing, DateTime expiration)
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


    }
}
