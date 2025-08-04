using Microsoft.IdentityModel.Tokens;
using Auth.Configuration.Redis.Repository.Interfaces;
using Auth.JWT.DTOs;
using Auth.JWT.Interfaces;
using Auth.Utils.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Server.Users.Model;
using Auth.JWT.Helper.Interfaces;

namespace Auth.JWT
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly IHttpContextAccessor _context;
        private readonly IRedisRepository _redisRepository;
        private readonly ITokenCreationServices _tokenCreation;

        public JwtService(IConfiguration configuration, IHttpContextAccessor context, IRedisRepository redisRepository, ITokenCreationServices tokenCreation)
        {
            var secretKeySection = configuration.GetSection("JwtSettings").GetSection("seecretKey").ToString();

            if (secretKeySection == null || string.IsNullOrEmpty(secretKeySection))
                throw new ArgumentNullException(nameof(secretKeySection), "Secret key cannot be null or empty");

            _secretKey = secretKeySection;
            _context = context;
            this._redisRepository = redisRepository;
            this._tokenCreation = tokenCreation;
        }

        /// <summary>
        /// generate token to validate email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateEmailVerificationOTT(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescription = this._tokenCreation.TokenDescriptionTemplate(user, "email_verification", DateTime.UtcNow.AddMinutes(10));

            var token = tokenHandler.CreateToken(tokenDescription);
            var ott = tokenHandler.WriteToken(token);

            await this._redisRepository.SetAsync(ott);
            return ott;

        }

        /// <summary>
        /// Generate RBA Token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ip"></param>
        /// <param name="userAgent"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<string> GenerateRBAOTT(UserModel user, string ip, string userAgent, string location)
        {
            var key = Encoding.UTF8.GetBytes(this._secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim ("sub", user.Id.ToString()),
                    new Claim ("ip", ip),
                    new Claim ("ua", userAgent),
                    new Claim ("location", location),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);
            var ott = tokenHandler.WriteToken(token);

            await this._redisRepository.SetAsync(ott);

            return ott;
        }

        /// <summary>
        /// Generate Recuperation Password Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateRecuperationPasswordOTT(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescription = this._tokenCreation.TokenDescriptionTemplate(user, "password_recuperation", DateTime.UtcNow.AddMinutes(10));

            var token = tokenHandler.CreateToken(tokenDescription);
            var ott = tokenHandler.WriteToken(token);

            await this._redisRepository.SetAsync(ott);
            return ott;
        }

        /// <summary>
        /// Get Claim From Token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public IEnumerable<Claim> GetClaimFromToken()
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("Http Context is null");

            var token = httpContext.Request.Cookies["Authentication"] ??
                throw new UnauthorizedAccessException("Token not found");

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var claim = jwtToken?.Claims ??
                throw new UnauthorizedAccessException("Invalid Token");

            return claim;
        }

        /// <summary>
        /// Generate Token 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TokenPair GenerateAuthenticationToken(int sessionId, UserModel user)
        {
            return this._tokenCreation.CreateTokenPair(
                sessionId,
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
        public TokenPair GenerateRefreshToken(int sessionId, UserModel user)
        {
            return this._tokenCreation.CreateTokenPair(
                sessionId,
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
        public bool IsTokenExpirationSoon(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token)) return false;

            if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken) return false;

            var expiration = jwtToken.ValidTo;

            return expiration <= DateTime.UtcNow.AddMinutes(60);
        }

        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ValidateAuthenticationToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(this._secretKey);

            if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken) return false;

            if (jwtToken.ValidTo < DateTime.UtcNow) return false;

            var principal = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            tokenHandler.ValidateToken(token, principal, out _);
            return true;
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
        public string GetValuesFromClaim(IEnumerable<Claim> claims, string type)
        {
            var value = claims.FirstOrDefault(c => c.Type == type)?.Value;

            if (string.IsNullOrEmpty(value))
                throw new UnauthorizedAccessException($"Invalid tokne, missing claim: {type}");

            return value;
        }

    }
}
