using Microsoft.AspNetCore.Identity;
using Auth.Cookies.Interfaces;
using Auth.JWT.Interfaces;
using Auth.Module.DTOs;
using Auth.Module.Services.Interfaces;
using Auth.Server.Users.DTOs;
using Auth.Utils.Exceptions;
using Auth.Queues.Messaging.Interfaces;
using Auth.Configuration.Redis.Repository.Interfaces;
using Auth.Utils.Helper;
using Auth.Server.Users.Model;
using Auth.Server.Users.Service.Interfaces;
using Auth.Server.Security.Service.Interfaces;
using Auth.Server.Security.Model;
using Auth.Module.Repository.Interface;
using Auth._2FA.Interfaces;
using Auth.Server.Hangfire.Interfaces;

namespace Auth.Module.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _context;
        private readonly IAuthRepository repository;
        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IUserService _userService;
        private readonly IMessagingQueues _messagingQueues;
        private readonly IRedisRepository _redisRepository;
        private readonly CodeGeneration codeGeneration;
        private readonly IValidateTwoFactorAuth validateTwoFactor;
        private readonly ISecurityService sessionService;
        private readonly IHangFireService hangFireService;

        public AuthService(IHttpContextAccessor context, IAuthRepository repository, IJwtService jwtService, ICookieService cookieService, IUserService userService, IMessagingQueues messagingQueues, IRedisRepository redisRepository, CodeGeneration codeGeneration, IValidateTwoFactorAuth validateTwoFactor, ISecurityService securityService, IHangFireService hangFireService)
        {
            this._context = context;
            this.repository = repository;
            this._jwtService = jwtService;
            this._cookieService = cookieService;
            this._userService = userService;
            this._messagingQueues = messagingQueues;
            this._redisRepository = redisRepository;
            this.codeGeneration = codeGeneration;
            this.validateTwoFactor = validateTwoFactor;
            this.sessionService = securityService;
            this.hangFireService = hangFireService;
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
            await this.repository.SaveAuth(user.Id);

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
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("httpContext is null");

            var claim = this._jwtService.GetClaimFromToken();
            var id = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));
            var sessionId = int.Parse(this._jwtService.GetClaimsValue(claim, "sessionId"));

            var user = await this._userService.GetUserById(id);
            var auth = await this.repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();

            var token = this._jwtService.RefreshToken(sessionId, user);
            auth.RefreshToken = token.RefreshHasherToken;

            await this.repository.UpdateAsync(auth);

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

            var auth = await this.repository.FindAuthByUserId(user.Id) ??
                throw new UnauthorizedAccessException();

            var code = this.codeGeneration.InvokeCodeGeneration();

            var currentIp = httpContext.Connection.RemoteIpAddress?.ToString();
            var currentUserAgent = httpContext.Request.Headers.UserAgent.ToString();

            using var client = new HttpClient();
            var response = await client.GetFromJsonAsync<IpInfoResponse>($"https://ipinfo.io/{currentIp}/json");
            var location = response.City ?? "Unkown";

            var sessions = await this.sessionService.FindAllSessionsByUserId(user.Id);

            if (sessions != null && sessions.Any())
            {
                bool sessionsExists = sessions.Any(s => s.Ip == currentIp && s.UserAgent == currentUserAgent);
                if (!sessionsExists)
                {

                    var token = await this._jwtService.GenerateRBAToken(user, currentIp, currentUserAgent, location);
                    await this._messagingQueues.RiskBasedAuthenticationMessage(token, user.Email, currentUserAgent, location);

                    return $"Check your email if you are really are you: IP {currentIp} UserAgent {currentUserAgent}";
                }
            }
            auth.TwoFACode = code;
            auth.TwoFACodeExpiration = DateTime.UtcNow.AddMinutes(10);

            await this.repository.UpdateAsync(auth);

            await this._messagingQueues.TowAfCodeMessage(user.Email, code);

            return $"Check your email to singing code";
        }

        /// <summary>
        /// Verify Session
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<string> VerifySession(string token)
        {
            var jwt = await this._jwtService.ValidateVerificationToken(token);

            var code = this.codeGeneration.InvokeCodeGeneration();

            var claims = jwt?.Claims ??
               throw new UnauthorizedAccessException("Invalid Token");

            var userId = this._jwtService.GetClaimsValue(claims, "sub");
            var ip = this._jwtService.GetClaimsValue(claims, "ip");
            var userAgent = this._jwtService.GetClaimsValue(claims, "ua");
            var location = this._jwtService.GetClaimsValue(claims, "location");

            int id = int.Parse(userId);

            await this.sessionService.SaveSession(id, ip, userAgent, location);

            var user = await this._userService.GetUserById(id);
            var auth = await this.repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();

            auth.TwoFACode = code;
            auth.TwoFACodeExpiration = DateTime.UtcNow.AddMinutes(10);
            await this.repository.UpdateAsync(auth);

            await this._messagingQueues.TowAfCodeMessage(user.Email, code);

            await this._redisRepository.UpdateStateAsync(token);

            return "Your new session was saved successfully, now you cann init session";
        }

        /// <summary>
        /// Init Session
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<string> InitSession(VerifyCodeDTO dto)
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("Http Context is null");

            using var client = new HttpClient();

            var ip = httpContext.Connection.RemoteIpAddress.ToString();
            // var ip = "8.8.8.8";
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            var response = await client.GetFromJsonAsync<IpInfoResponse>($"https://ipinfo.io/{ip}/json");
            var location = response.City ?? "Unkown";

            var user = await this._userService.GetUserByEmail(dto.Email);
            var auth = await this.validateTwoFactor.ImplementValidate(user.Id, dto.TwoAfCode);

            var session = await this.sessionService.SessionExist(user.Id, ip, userAgent, location);
            int sessionId = session != null
                ? session.Id
                : await this.sessionService.SaveSession(user.Id, ip, userAgent, location);

            var token = this._jwtService.GenerateToken(sessionId, user);

            auth.RefreshToken = token.RefreshHasherToken;
            await this.repository.UpdateAsync(auth);

            this._cookieService.SetTokenCookies(httpContext.Response, token);

            return $"Welcome {user.FullName}";
        }

        /// <summary>
        /// Get User By Cookie
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AuthorizationTokenDTO> GetValueByCookie()
        {
            var claim = this._jwtService.GetClaimFromToken();
            int id = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));
            int sessionId = int.Parse(this._jwtService.GetClaimsValue(claim, "sessionId"));

            var user = await this._userService.GetUserById(id);
            var response = new AuthorizationTokenDTO { User = user, SessionId = sessionId };

            return response;
        }

        /// <summary>
        /// List of all session of user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotFoundExceptions"></exception>
        public async Task<IEnumerable<SessionModel?>> ListOfAllSessionsAsync()
        {
            var claim = this._jwtService.GetClaimFromToken();
            int id = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));

            var sessions = await this.sessionService.FindAllSessionsByUserId(id) ??
                throw new NotFoundExceptions("User not found");

            return sessions;
        }
        /// <summary>
        /// Remove one session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> RemoveOneSessionById(int id)
        {
            var claim = this._jwtService.GetClaimFromToken();
            int userId = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));

            var sessions = await this.sessionService.FindAllSessionsByUserId(userId);

            if (!sessions.Any(s => s?.Id == id))
                throw new UnauthorizedAccessException("Your not allowed to deleted this session");

            if (id == int.Parse(this._jwtService.GetClaimsValue(claim, "sessionId")))
                throw new UnauthorizedAccessException("You cannot delete the session you are using.");

            await this.sessionService.RemoveSessionById(id);

            return "This session was deleted successfully";
        }

        /// <summary>
        /// Forget Password
        /// </summary>
        /// <param name="dto"></param>
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

            var claim = this._jwtService.GetClaimFromToken();
            int id = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));

            var auth = await this.repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();
            auth.RefreshToken = null;

            await this.repository.UpdateAsync(auth);

            this._cookieService.ClearTokenCookies(httpContext.Response);
        }

        /// <summary>
        ///  Remove Own Account
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestExceptions"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ForbiddenExceptions"></exception>
        public async Task<string> RemoveOwnAccount(RemoveOwnAccountDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Password))
                throw new BadRequestExceptions("Password is required");

            var claim = this._jwtService.GetClaimFromToken();
            int id = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));

            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("HttpContext is null");

            var user = await this._userService.GetUserById(id);
            var passwordHasher = new PasswordHasher<UserModel>();
            var verificationPass = passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (verificationPass == PasswordVerificationResult.Failed)
                throw new ForbiddenExceptions("Password is Wrong");

            var auth = await this.validateTwoFactor.ImplementValidate(id, dto.Code);

            auth.IsDeleted = true;
            auth.DeletedAt = DateTime.UtcNow;
            auth.ScheduledDeletionJobId = this.hangFireService.ScheduleIdKey(id);
            auth.RefreshToken = null;

            await this.repository.UpdateAsync(auth);

            this._cookieService.ClearTokenCookies(httpContext.Response);

            return "Your account will be deleted in the next 10 minutes.";
        }

        /// <summary>
        /// Validate Refresh Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task RefreshTokenValidate(string refreshToken, int id)
        {
            var auth = await this.repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();

            var match = BCrypt.Net.BCrypt.Equals(refreshToken, auth.RefreshToken);
            if (!match) throw new UnauthorizedAccessException("Refresh Token is invalid");
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
            var auth = await this.repository.FindAuthByUserId(id) ??
                throw new UnauthorizedAccessException();

            auth.EmailVerify = true;
            await this.repository.UpdateAsync(auth);

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
            var auth = await this.repository.FindAuthByUserId(id) ??
                throw new UnauthorizedAccessException();

            auth.EmailVerify = true;
            await this.repository.UpdateAsync(auth);

            await this._redisRepository.UpdateStateAsync(token);

            return $"{user.Username} your new adress was verificate successfully, now you can login in.";
        }

        /// <summary>
        /// Generate 2FA Code
        /// </summary>
        /// <returns></returns>
        public async Task<string> TwoFactorAuthenticationCodeGeneration()
        {
            var claim = this._jwtService.GetClaimFromToken();
            int id = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));

            var user = await this._userService.GetUserById(id);
            var auth = await this.repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();

            var code = this.codeGeneration.InvokeCodeGeneration();

            auth.TwoFACode = code;
            auth.TwoFACodeExpiration = DateTime.UtcNow.AddMinutes(10);
            await this.repository.UpdateAsync(auth);

            await this._messagingQueues.TowAfCodeMessage(user.Email, code);

            return $"Check your email to singing code";
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<string> ChangePassword(UpdatePasswordDTO body)
        {
            var claim = this._jwtService.GetClaimFromToken();

            int id = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));
            int sessionId = int.Parse(this._jwtService.GetClaimsValue(claim, "sessionId"));

            await this.validateTwoFactor.ImplementValidate(id, body.Code);

            var response = await this._userService.UpdatePasswordUser(id, body);

            await this.sessionService.RemoveAllSessionExceptCurrent(id, sessionId);

            return response;
        }

        /// <summary>
        /// Update Email Address
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<string> ChangeAddressEmail(NewEmailDTO body)
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException();

            var claim = this._jwtService.GetClaimFromToken();

            int id = int.Parse(this._jwtService.GetClaimsValue(claim, "sub"));
            int sessionId = int.Parse(this._jwtService.GetClaimsValue(claim, "sessionId"));

            var auth = await this.validateTwoFactor.ImplementValidate(id, body.Code);

            var user = await this._userService.UpdateEmailAddress(id, body) ?? 
                throw new BadRequestExceptions("The email could not be updated.");

            var ott = await this._jwtService.GenerateEmailVerificationToken(user);
            await this._messagingQueues.SendNewEmailVerificationEvent(user.Email, ott, user.Id);

            await this.sessionService.RemoveAllSessionExceptCurrent(id, sessionId);

            auth.EmailVerify = false;
            auth.RefreshToken = null;
            await this.repository.UpdateAsync(auth);

            this._cookieService.ClearTokenCookies(httpContext.Response);

            return $"Email was updated his new email is {user?.Email} ";
        }

        /// <summary>
        /// Validate User Credentials
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="TooManyRequestsException"></exception>
        /// <exception cref="ForbiddenExceptions"></exception>
        public async Task<UserModel> ValidateUserCredentials(LoginDTO body)
        {
            var user = await this._userService.FindByValue("Username", body.Username) ??
                throw new KeyNotFoundException("This Username is wrong or not was registered");

            var auth = await this.repository.FindAuthByUserId(user.Id) ?? throw new UnauthorizedAccessException();

            var passwordHaser = new PasswordHasher<UserModel>();
            var verificationResult = passwordHaser.VerifyHashedPassword(user, user.Password, body.Password);

            var shouldUpdate = false;

            if (auth.LockedAt != null && DateTime.UtcNow < auth.LockedAt)
                throw new TooManyRequestsException("Account locked due to multiple failed attemps. Try again later");

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                auth.VerifyAttempts++;

                if (auth.VerifyAttempts >= 3)
                {
                    auth.LockedAt = DateTime.UtcNow.AddMinutes(10);
                    auth.VerifyAttempts = 0;
                }
                await this.repository.UpdateAsync(auth);
                throw new UnauthorizedAccessException("Password is wrong");
            }

            if (auth.VerifyAttempts > 0 || auth.LockedAt != null)
            {
                auth.VerifyAttempts = 0;
                auth.LockedAt = null;
                shouldUpdate = true;
            }

            if (!auth.EmailVerify)
                throw new ForbiddenExceptions("You need check your email to login");

            if (auth.IsDeleted)
            {
                auth.IsDeleted = false;
                auth.DeletedAt = null;

                this.hangFireService.DeletedScheduledJob(auth.ScheduledDeletionJobId);
                auth.ScheduledDeletionJobId = null;

                shouldUpdate = true;
            }

            if (shouldUpdate)
                await this.repository.UpdateAsync(auth);

            return user;
        }
    }
}
