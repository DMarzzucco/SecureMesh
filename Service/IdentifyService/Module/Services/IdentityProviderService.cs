using Microsoft.AspNetCore.Identity;
using IdentifyService.Cookies.Interfaces;
using IdentifyService.JWT.Interfaces;
using IdentifyService.Module.DTOs;
using IdentifyService.Module.Services.Interfaces;
using IdentifyService.Server.UMS.DTOs;
using IdentifyService.Utils.Exceptions;
using IdentifyService.Queues.Messaging.Interfaces;
using IdentifyService.Configuration.Redis.Repository.Interfaces;
using IdentifyService.Utils.Helper;
using IdentifyService.Server.UMS.Model;
using IdentifyService.Module.Repository.Interface;
using IdentifyService._2FA.Interfaces;
using IdentifyService.Server.UMS.Services.Interfaces;
using IdentifyService.Utils.Helper.IpService.Interfaces;

namespace IdentifyService.Module.Services
{
    public class IdentityProviderService : IIdentityProviderService
    {
        private readonly IHttpContextAccessor _context;
        private readonly IIdentityProviderRepository _repository;
        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IMessagingQueues _messagingQueues;
        private readonly IRedisRepository _redisRepository;
        private readonly CodeGeneration _codeGeneration;
        private readonly IValidateTwoFactorAuth _validateTwoFactor;
        private readonly IManagementUserFacedeServices _managementUser;
        private readonly IIpService _ipService;

        public IdentityProviderService(IHttpContextAccessor context, IIdentityProviderRepository repository, IJwtService jwtService, ICookieService cookieService, IMessagingQueues messagingQueues, IRedisRepository redisRepository, CodeGeneration codeGeneration, IValidateTwoFactorAuth validateTwoFactor, IManagementUserFacedeServices managementUser, IIpService ipService)
        {
            _context = context;
            _repository = repository;
            _jwtService = jwtService;
            _cookieService = cookieService;
            _messagingQueues = messagingQueues;
            _redisRepository = redisRepository;
            _codeGeneration = codeGeneration;
            _validateTwoFactor = validateTwoFactor;
            _managementUser = managementUser;
            _ipService = ipService;
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

            var user = await this._managementUser.SaveUserRegistered(body);
            await this._repository.SaveAuth(user.Id);

            if (user != null)
            {
                var verificationToken = await this._jwtService.GenerateEmailVerificationOTT(user);

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
            var id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));
            var sessionId = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sessionId"));

            var user = await this._managementUser.FindUserById(id);
            var idp = await this._repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();

            var token = this._jwtService.GenerateRefreshToken(sessionId, user);
            idp.RefreshToken = token.RefreshHasherToken;

            await this._repository.UpdateAsync(idp);

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

            var idp = await this._repository.FindAuthByUserId(user.Id) ??
                throw new UnauthorizedAccessException();

            var code = this._codeGeneration.InvokeCodeGeneration();

            var currentIp = httpContext.Connection.RemoteIpAddress?.ToString();
            var currentUserAgent = httpContext.Request.Headers.UserAgent.ToString();

            using var client = new HttpClient();
            var response = await client.GetFromJsonAsync<IpInfoResponse>($"https://ipinfo.io/{currentIp}/json");
            var location = response?.City ?? "Unkown";

            var sessions = await this._managementUser.FindAllSessionsByUserId(user.Id);

            if (sessions != null && sessions.Any())
            {
                bool sessionsExists = sessions.Any(s => s.Ip == currentIp && s.UserAgent == currentUserAgent);
                if (!sessionsExists)
                {

                    var token = await this._jwtService.GenerateRBAOTT(user, currentIp, currentUserAgent, location);
                    await this._messagingQueues.RiskBasedAuthenticationMessage(token, user.Email, currentUserAgent, location);

                    return $"Check your email if you are really are you: IP {currentIp} UserAgent {currentUserAgent}";
                }
            }
            idp.TwoFACode = code;
            idp.TwoFACodeExpiration = DateTime.UtcNow.AddMinutes(10);

            await this._repository.UpdateAsync(idp);

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
            var jwt = await this._jwtService.ValidateOTT(token);

            var code = this._codeGeneration.InvokeCodeGeneration();

            var claims = jwt?.Claims ??
               throw new UnauthorizedAccessException("Invalid Token");

            var userId = this._jwtService.GetValuesFromClaims(claims, "sub");
            var ip = this._jwtService.GetValuesFromClaims(claims, "ip");
            var userAgent = this._jwtService.GetValuesFromClaims(claims, "ua");
            var location = this._jwtService.GetValuesFromClaims(claims, "location");

            int id = int.Parse(userId);

            await this._managementUser.SaveSessionRegister(id, ip, userAgent, location);

            var user = await this._managementUser.FindUserById(id);
            var idp = await this._repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();

            idp.TwoFACode = code;
            idp.TwoFACodeExpiration = DateTime.UtcNow.AddMinutes(10);
            await this._repository.UpdateAsync(idp);

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

            var ip = httpContext?.Connection?.RemoteIpAddress?.ToString();
            // var ip = "8.8.8.8";
            var userAgent = httpContext?.Request.Headers.UserAgent.ToString();

            var location = await this._ipService.GetCityAsync(ip);

            var user = await this._managementUser.FindUserByEmail(dto.Email);
            var idp = await this._validateTwoFactor.ImplementValidate(user.Id, dto.TwoAfCode);

            var session = await this._managementUser.FindSessionIfExists(user.Id, ip, userAgent, location);
            int sessionId = session != null
                ? session.Id
                : await this._managementUser.SaveSessionRegister(user.Id, ip, userAgent, location);

            var token = this._jwtService.GenerateAuthenticationToken(sessionId, user);

            idp.RefreshToken = token.RefreshHasherToken;
            await this._repository.UpdateAsync(idp);

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
            int id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));
            int sessionId = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sessionId"));

            var user = await this._managementUser.FindUserById(id);
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
            int id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));

            var sessions = await this._managementUser.FindAllSessionsByUserId(id) ??
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
            int userId = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));

            var session = await this._managementUser.FindAllSessionsByUserId(userId);

            if (!session.Any(s => s?.Id == id))
                throw new UnauthorizedAccessException("Your not allowed to deleted this session");

            if (id == int.Parse(this._jwtService.GetValuesFromClaims(claim, "sessionId")))
                throw new UnauthorizedAccessException("You cannot delete the session you are using.");

            await this._managementUser.RemoveSessionById(id);

            return "This session was deleted successfully";
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
            int id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));

            var idp = await this._repository.FindAuthByUserId(id) ??
                throw new UnauthorizedAccessException();

            idp.RefreshToken = null;

            await this._repository.UpdateAsync(idp);

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
            int id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));

            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException("HttpContext is null");

            var idp = await this._validateTwoFactor.ImplementValidate(id, dto.Code);

            var user = await this._managementUser.FindUserById(id);

            var passwordHasher = new PasswordHasher<UserModel>();
            var verificationPass = passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (verificationPass == PasswordVerificationResult.Failed)
                throw new ForbiddenExceptions("Password is Wrong");

            await this._managementUser.RequestToRemoveOwnAccount(id);

            idp.RefreshToken = null;

            await this._repository.UpdateAsync(idp);

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
            var idp = await this._repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();

            var match = BCrypt.Net.BCrypt.Equals(refreshToken, idp.RefreshToken);
            if (!match) throw new UnauthorizedAccessException("Refresh Token is invalid");
        }


        /// <summary>
        /// VerificationEmail
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> VerificationEmail(string token)
        {
            var jwt = await this._jwtService.ValidateOTT(token);

            var claim = jwt?.Claims ?? throw new UnauthorizedAccessException("Invalid Token");

            int id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));

            var user = await this._managementUser.FindUserById(id);
            var idp = await this._repository.FindAuthByUserId(id) ??
                throw new UnauthorizedAccessException();

            idp.EmailVerify = true;
            await this._repository.UpdateAsync(idp);

            await this._messagingQueues.SendWelcomeMessage(user.FullName, user.Email, user.Id);
            await this._redisRepository.UpdateStateAsync(token);

            return $"Hello {user.FullName} your account was verificate successfully.";
        }

        /// <summary>
        /// Generate 2FA Code
        /// </summary>
        /// <returns></returns>
        public async Task<string> TwoFactorAuthenticationCodeGeneration()
        {
            var claim = this._jwtService.GetClaimFromToken();
            int id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));

            var user = await this._managementUser.FindUserById(id);
            var idp = await this._repository.FindAuthByUserId(id) ?? throw new UnauthorizedAccessException();

            var code = this._codeGeneration.InvokeCodeGeneration();

            idp.TwoFACode = code;
            idp.TwoFACodeExpiration = DateTime.UtcNow.AddMinutes(10);
            await this._repository.UpdateAsync(idp);

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

            int id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));
            int sessionId = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sessionId"));

            await this._validateTwoFactor.ImplementValidate(id, body.Code);

            var response = await this._managementUser.UpdatePasswordUser(id, sessionId, body);

            return response;
        }

        /// <summary>
        /// Solicit changes address email
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="BadRequestExceptions"></exception>
        public async Task<string> ChangeAddressEmail(NewEmailDTO body)
        {
            var httpContext = this._context.HttpContext ??
                throw new UnauthorizedAccessException();

            var claim = this._jwtService.GetClaimFromToken();

            int id = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sub"));
            int sessionId = int.Parse(this._jwtService.GetValuesFromClaims(claim, "sessionId"));

            var idp = await this._validateTwoFactor.ImplementValidate(id, body.Code);

            if (string.IsNullOrEmpty(body.NewEmail))
                throw new BadRequestExceptions("Email not be null");

            var user = await this._managementUser.VerifyNewEmailParameters(id, body);

            var token = await this._jwtService.GenerateVerifyNewEmailOTT(user, sessionId, body.NewEmail);
            await this._messagingQueues.SendNewEmailVerificationEvent(user.Email, token, user.Id);

            idp.RefreshToken = null;
            await this._repository.UpdateAsync(idp);

            this._cookieService.ClearTokenCookies(httpContext.Response);

            return $"To complete this process, please check your email at {user?.Email} to verify it.";
        }

        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="TooManyRequestsException"></exception>
        /// <exception cref="ForbiddenExceptions"></exception>
        public async Task<UserModel> ValidateUserCredential(LoginDTO body)
        {
            var user = await this._managementUser.FindByValue(body.Username);

            var idp = await this._repository.FindAuthByUserId(user.Id) ?? throw new UnauthorizedAccessException();

            var passwordHaser = new PasswordHasher<UserModel>();
            var verificationResult = passwordHaser.VerifyHashedPassword(user, user.Password, body.Password);


            if (idp.LockedAt != null && DateTime.UtcNow < idp.LockedAt)
                throw new TooManyRequestsException("Account locked due to multiple failed attemps. Try again later");

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                idp.VerifyAttempts++;
                if (idp.VerifyAttempts >= 3)
                {
                    idp.LockedAt = DateTime.UtcNow.AddMinutes(10);
                    idp.VerifyAttempts = 0;
                }
                await this._repository.UpdateAsync(idp);
                throw new UnauthorizedAccessException("Password is wrong");
            }

            if (idp.VerifyAttempts > 0 || idp.LockedAt != null)
            {
                idp.VerifyAttempts = 0;
                idp.LockedAt = null;
                await this._repository.UpdateAsync(idp);
            }

            if (!idp.EmailVerify)
                throw new ForbiddenExceptions("You need check your email to login");

            await this._managementUser.CancelRemoveAccountOperationIfOn(user.Id);

            return user;
        }
    }
}
