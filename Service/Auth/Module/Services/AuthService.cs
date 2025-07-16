using Microsoft.AspNetCore.Identity;
using Auth.Cookies.Interfaces;
using Auth.JWT.Interfaces;
using Auth.Module.DTOs;
using Auth.Module.Services.Interfaces;
using Auth.Server.DTOs;
using Auth.Server.Model;
using Auth.Server.Service.Interfaces;
using Auth.Utils.Exceptions;
using Auth.Queues.Messaging.Interfaces;
using Auth.Configuration.Redis.Repository.Interfaces;
using Auth.Utils.Helper;

namespace Auth.Module.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _context;
        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IUserService _userService;
        private readonly IMessagingQueues _messagingQueues;
        private readonly IRedisRepository _redisRepository;
        private readonly CodeGeneration codeGeneration;

        public AuthService(IHttpContextAccessor context, IJwtService jwtService, ICookieService cookieService, IUserService userService, IMessagingQueues messagingQueues, IRedisRepository redisRepository, CodeGeneration codeGeneration)
        {
            this._context = context;
            this._jwtService = jwtService;
            this._cookieService = cookieService;
            this._userService = userService;
            this._messagingQueues = messagingQueues;
            this._redisRepository = redisRepository;
            this.codeGeneration = codeGeneration;
        }
        /// <summary>
        /// Registered of user
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestExceptions"></exception>
        public async Task<string> RegisteredUser(CreateUserDTO body)
        {
            if (body == null)
                throw new BadRequestExceptions($"{body} is required");

            var user = await this._userService.RegisterUser(body);
            if (user != null)
            {
                var verificationToken = await this._jwtService.GenerateEmailVerificationToken(user);

                await this._messagingQueues.SendEmailVerificactionEvent(user.Email, verificationToken, user.Id);
            }

            return $"Your was registerd successfully, now you need check your email to verificated";
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
        /// Login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> Login(UserModel user)
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("Http Context is null");

            var code = this.codeGeneration.InvokeCodeGeneration();
            var expiration = DateTime.UtcNow.AddMinutes(10);

            var currentIp = httpContext.Connection.RemoteIpAddress?.ToString();
            var currentUserAgent = httpContext.Request.Headers.UserAgent.ToString();

            /// var relation = await this.securityService.FindSessionByUserId(body.Id);

            ///if(relation != null)
            ///{
            /// if (realtion.Ip != currentIp || realtion.UserAent != currentUserAgent)
            /// {
            /// 
            ///  var token = await this._jwtService.GenerateTokenRBA(user);
            ///  await this._messagingQueues.SendRBAMessage(token, email, realation.IP, relation.UserAgent, etc);
            /// 
            ///  return $"Check your email if you are really IP {currentIp} UserAgent {currentUserAgent}";
            /// }
            ///} 

            await this._userService.UpdateTwoAFCode(user.Id, code, expiration);
            await this._messagingQueues.TowAfCodeMessage(user.Email, code);


            return $"Check your email to singing code";
        }

        /// <summary>
        /// Verify 2AF Code
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<string> VerifyTowAFCode(VerifyCodeDTO dto)
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("Http Context is null");

            var user = await this._userService.VerifyTwoAF(dto.Email, dto.TwoAfCode);

            var token = this._jwtService.GenerateToken(user);
            await this._userService.UpdateRefreshToken(user.Id, token.RefreshHasherToken);

            this._cookieService.SetTokenCookies(httpContext.Response, token);

            // await this.securityService.SaveSession(user.Id);

            return $"Welcome {user.FullName}";
        }

        /// public async Task<string> VerifySession (string token)
        /// {
        ///  var jwt = await this._jwtService.ValidateVerifyToken(token);
        ///  var userId = jwt?.Claims.FirstOrDefault(c=> c.Type == "sub")?.Value ??
        ///     throw new UnathorizedAccessException ("Invalid Token");
        /// 
        ///  int id = int.Parse(userId);
        /// 
        ///  await this.securityService.SaveSession (id);
        ///  await this._redisRepository.UpdateStateAsync(token);
        ///  
        ///  return "Your new session was saved successfully";
        /// }
        /// NOT ALLOWEED
        /// 
        /// public async Task<string> NotAllowedSession (string token)
        /// {
        ///  var jwt = await this._jwtService.ValidateVerifyToken(token);
        ///  var userId = jwt?.Claims.FirstOrDefault(c=> c.Type == "sub")?.Value ??
        ///     throw new UnathorizedAccessException ("Invalid Token");
        /// 
        ///  int id = int.Parse(userId);
        ///  
        ///  await this.securityService.DenegatePermision(id);
        ///  await this.redisRepository.UpdateStateAsync(token);
        ///  return "Session denegate, you must be change your password";
        /// }
        
        
        /// <summary>
        /// Get Profile
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<object> GetProfile()
        {
            var user = await this.GetUserByCookie();
            var response = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                Roles = user.Roles
            };
            return response;
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
        /// Forget Password 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<string> ForgetPassword(ForgetPasswordDTO dto)
        {
            var user = await this._userService.GetUserByEmail(dto.Email);
            if (user != null)
            {
                var token = await this._jwtService.GenerateRecuperationPasswordToken(user);
                await this._messagingQueues.PasswordRecuperationMessage(user.Email, token, user.Id);
            }
            return "You need check your email to next.";
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
        /// Remove Own Account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<string> RemoveOwnAccount(int id, PasswordDTO dto)
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("HttpContext is null");
            var user = await this._userService.GetUserById(id);

            var response = await this._userService.DeletedOwnAccount(id, dto);
            // await this.LogOut();

            await this._userService.UpdateRefreshToken(user.Id, null);
            this._cookieService.ClearTokenCookies(httpContext.Response);

            return response;
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
        /// Reset Password
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<string> ResetPassword(string token, PasswordDTO body)
        {
            var jwtToken = await this._jwtService.ValidateVerificationToken(token);

            var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                throw new UnauthorizedAccessException("Invalid Token");

            int id = int.Parse(userId);

            var user = await this._userService.ReturnPassword(id, body);

            //invalidar token
            await this._redisRepository.UpdateStateAsync(token);

            return $"{user.FullName} Your new password was chanchis successfully";
        }


        /// <summary>
        /// VerificationEmail
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> VerificationEmail(string token)
        {
            var jwt = await this._jwtService.ValidateVerificationToken(token);

            var userId = jwt?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                throw new UnauthorizedAccessException("Invalid Token");

            int id = int.Parse(userId);

            var user = await this._userService.GetUserById(id);

            await this._userService.MarkEmailAsync(id);
            await this._messagingQueues.SendWelcomeMessage(user.FullName, user.Email, user.Id);
            await this._redisRepository.UpdateStateAsync(token);

            return $"Hello {user.FullName} your account was verificate successfully.";
        }

        /// <summary>
        /// Validate New Email
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> VerificationNewEmail(string token)
        {
            var jwt = await this._jwtService.ValidateVerificationToken(token);

            var userId = jwt?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                throw new UnauthorizedAccessException("Invalid Token");

            int id = int.Parse(userId);

            var user = await this._userService.GetUserById(id);

            await this._userService.MarkEmailAsync(id);
            await this._redisRepository.UpdateStateAsync(token);

            return $"{user.Username} your new adress was verificate successfully, now you can login in.";
        }

        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ForbiddenExceptions"></exception>
        public async Task<UserModel> ValidateUser(LoginDTO body)
        {
            // var httpContext = this._context.HttpContext ??
            //     throw new UnauthorizedAccessException("Http Context is null");

            // var csrfFromHeader = httpContext.Request.Headers["X-XSRF-TOKEN"];

            var user = await this._userService.FindByValue("Username", body.Username) ??
                throw new KeyNotFoundException("This Username is wrong or not was registered");

            var passwordHaser = new PasswordHasher<UserModel>();

            var verificationResult = passwordHaser.VerifyHashedPassword(user, user.Password, body.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Password is wrong");

            if (!user.EmailVerified)
                throw new ForbiddenExceptions("You need check your email to login");

            // if (user.CsrfToken == null || user.CsrfTokenExpiration < DateTime.UtcNow)
            // {
            //     var csrfToken = Guid.NewGuid().ToString("N");
            //     var csrfTokenHashed = BCrypt.Net.BCrypt.HashPassword(csrfToken);
            //     DateTime csrfTokenExpiration = DateTime.UtcNow.AddMinutes(30);

            //     await this._userService.UpdateCsrfToken(user.Id, csrfTokenHashed, csrfTokenExpiration);
            //     this._cookieService.SetCRSFToken(httpContext.Response, "XSRF-TOKEN", csrfToken);
            // }
            // else
            // {
            //     if (string.IsNullOrEmpty(csrfFromHeader) || !BCrypt.Net.BCrypt.Verify(csrfFromHeader, user.CsrfToken))
            //         throw new ForbiddenExceptions("Unauthorized request.");
            // }

            await this._userService.CancelationOperation(user.Id);

            return user;
        }

        /// <summary>
        /// Update Email Address
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<string> ChangeAddressEmail(int id, NewEmailDTO body)
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("HttpContext is null");

            var user = await this._userService.UpdateEmailAdress(id, body);
            if (user != null)
            {
                var token = await this._jwtService.GenerateEmailVerificationToken(user);
                await this._messagingQueues.SendNewEmailVerificationEvent(user.Email, token, user.Id);

                // await this.LogOut();
                await this._userService.UpdateRefreshToken(user.Id, null);
                this._cookieService.ClearTokenCookies(httpContext.Response);
            }

            return $"Email was updated his new email is {user.Email} ";
        }
    }
}
