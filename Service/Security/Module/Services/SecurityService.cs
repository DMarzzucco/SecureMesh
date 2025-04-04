using Microsoft.AspNetCore.Identity;
using Security.Cookies.Interfaces;
using Security.JWT.Interfaces;
using Security.Module.DTOs;
using Security.Module.Services.Interfaces;
using Security.Server.Model;
using Security.Server.Service.Interfaces;

namespace Security.Module.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IHttpContextAccessor _context;
        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IUserService _userService;

        public SecurityService(IHttpContextAccessor context, IJwtService jwtService, ICookieService cookieService, IUserService userService)
        {
            _context = context;
            _jwtService = jwtService;
            _cookieService = cookieService;
            _userService = userService;
        }

        /// <summary>
        /// Generate Refresh Token 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> GenerateRefreshToken()
        {
            var httpContext = this._context.HttpContext ?? throw new UnauthorizedAccessException("httpContext is null");

            var user = await this.GetUserByCookie();
            var token = this._jwtService.RefreshToken(user);

            await this._userService.UpdateRefreshToken(user.Id, token.RefreshHasherToken);

            this._cookieService.SetTokenCookies(httpContext.Response, token);
            return token.AccessToken;
        }
        /// <summary>
        /// Generate Token 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> GenerateToken(UserModel body)
        {
            var httpContext = this._context.HttpContext ?? throw new UnauthorizedAccessException("Http Context is null");

            var token = this._jwtService.GenerateToken(body);
            await this._userService.UpdateRefreshToken(body.Id, token.RefreshHasherToken);
            this._cookieService.SetTokenCookies(httpContext.Response, token);

            return token.AccessToken;
        }
        /// <summary>
        /// Get Profile
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<object> GetProfile()
        {
            var user = await this.GetUserByCookie();
            return user;
        }
        /// <summary>
        /// Get User By Cookie
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel> GetUserByCookie()
        {
            var id = this._jwtService.GetIdFromToken();
            var user = await this._userService.GetUserById(id);
            return user;
        }
        /// <summary>
        /// Log Out
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task LogOut()
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("HttpContext is null");

            var user = await this.GetUserByCookie();
            await this._userService.UpdateRefreshToken(user.Id, null);

            this._cookieService.ClearTokenCookies(httpContext.Response);
        }
        /// <summary>
        /// Refresh Token Validate
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel> RefreshTokenValidate(string refreshToken, int id)
        {
            var user = await this._userService.GetUserById(id);

            var match = BCrypt.Net.BCrypt.Equals(refreshToken, user.RefreshToken);
            if (!match) throw new UnauthorizedAccessException("Refresh Token is invalid");

            return user;
        }
        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserModel> ValidateUser(LoginDTO body)
        {
            var user = await this._userService.FindByValue("Username", body.Username) ??
                throw new KeyNotFoundException("This Username is wrong or not was registered");

            var passwordHaser = new PasswordHasher<UserModel>();

            var verificationResult = passwordHaser.VerifyHashedPassword(user, user.Password, body.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Password is wrong");

            return user;
        }
    }
}
