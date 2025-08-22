using Microsoft.AspNetCore.Identity;
using UserManagementService.Configuration.Redis.Repository.Interfaces;
using UserManagementService.JWT.Services.Interfaces;
using UserManagementService.Modules.Repository.Interfaces;
using UserManagementService.Modules.Services.Interfaces;
using UserManagementService.Queues.Messaging.Interfaces;
using UserManagementService.Server.Sessions.Services.Interfaces;
using UserManagementService.Server.Users.Model;
using UserManagementService.Server.Users.Service.Interfaces;
using UserManagementService.Utils.Exceptions;

namespace UserManagementService.Modules.Services;

public class ManagementUserServices : IManagementUserServices
{
    private readonly IUserService _userService;
    private readonly ISessionManagementServices _sessionService;
    private readonly IJwtServices _jwtServices;
    private readonly IRedisRepository _redis;
    private readonly IManagementUserRepository _repository;
    private readonly IMessagingQueues _messagingQueues;

    public ManagementUserServices(IUserService userService, ISessionManagementServices sessionService, IJwtServices jwtServices, IRedisRepository redis, IMessagingQueues messagingQueues, IManagementUserRepository repository)
    {
        _userService = userService;
        _sessionService = sessionService;
        _jwtServices = jwtServices;
        _redis = redis;
        _messagingQueues = messagingQueues;
        _repository = repository;
    }

    /// <summary>
    /// Update any credential 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundExceptions"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<string> UpdateAnyCrendetial(int id, UpdateOwnRegisterDTO body)
    {
        var user = await this._userService.GetUserById(id);

        var passwordHasher = new PasswordHasher<UserModel>();
        var verifyPass = passwordHasher.VerifyHashedPassword(user, user.Password, body.Password);

        var ms = await this._repository.GetRelationManagementByUserId(id) ?? throw new NotFoundExceptions("User not found");

        if (ms.LockedAt != null && DateTime.UtcNow < ms.LockedAt)
            throw new UnauthorizedAccessException("Account locked due to multiple fails");

        if (verifyPass == PasswordVerificationResult.Failed)
        {
            ms.Attemps++;
            if (ms.Attemps >= 3)
            {
                ms.LockedAt = DateTime.UtcNow.AddMinutes(10);
                ms.Attemps = 0;
            }
            await this._repository.UpdateManagementUser(ms);
            throw new UnauthorizedAccessException("Password si wrong");
        }
        
        if (ms.Attemps > 0 || ms.LockedAt != null)
        {
            ms.Attemps = 0;
            ms.LockedAt = null;
            await this._repository.UpdateManagementUser(ms);
        }

        var response = await this._userService.UpdateOwnRegister(id, body);
        return response;
    }

    /// <summary>
    /// Update Email Adress
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="sessionId"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<string> UpdateEmailAdress(string ott)
    {
        var jwt = await this._jwtServices.ValidateOTT(ott);

        var claim = jwt?.Claims ??
            throw new UnauthorizedAccessException("Invalid Token");

        int userId = int.Parse(this._jwtServices.GetValuesFromClaims(claim, "sub"));
        int sessionId = int.Parse(this._jwtServices.GetValuesFromClaims(claim, "sessionId"));
        string newEmail = this._jwtServices.GetValuesFromClaims(claim, "new_email");

        var user = await this._userService.UpdateEmailAddress(userId, newEmail);
        await this._sessionService.RemoveAllSessionExceptCurrent(userId, sessionId);

        await this._redis.UpdateStateAsync(ott);

        return "Your new email address was updated";
    }

    /// <summary>
    /// Forget Password Account
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<string> ForgetPasswordAccount(ForgetPasswordDTO dto)
    {
        var user = await this._userService.GetUserByEmail(dto.Email);
        var ott = await this._jwtServices.GenerateRecuperationPasswordOTT(user);

        await this._messagingQueues.PasswordRecuperationMessage(user.Email, ott, user.Id);

        return "Check your email to continue with the operation";
    }

    /// <summary>
    /// Reset Password
    /// </summary>
    /// <param name="ott"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<string> ResetPassword(string ott, PasswordDTO body)
    {
        var jwt = await this._jwtServices.ValidateOTT(ott);

        var claim = jwt?.Claims ??
            throw new UnauthorizedAccessException("Invalid Token");

        int userId = int.Parse(this._jwtServices.GetValuesFromClaims(claim, "sub"));

        var user = await this._userService.ReturnPassword(userId, body);
        await this._sessionService.RemoveAllSessionsByUserId(userId);

        await this._redis.UpdateStateAsync(ott);

        return $"{user.FullName} your new password was chanches successfully";
    }
}
