using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using UmsTesting.Mock;
using UserManagementService.Configuration.Redis.Repository.Interfaces;
using UserManagementService.JWT.Services.Interfaces;
using UserManagementService.Modules.Models;
using UserManagementService.Modules.Repository.Interfaces;
using UserManagementService.Modules.Services;
using UserManagementService.Queues.Messaging.Interfaces;
using UserManagementService.Server.Sessions.Services.Interfaces;
using UserManagementService.Server.Users.Service.Interfaces;

namespace UmsTesting.Service;

public class UmsServiceTesting
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<ISessionManagementServices> _sessionService;
    private readonly Mock<IJwtServices> _jwtService;
    private readonly Mock<IRedisRepository> _redis;
    private readonly Mock<IManagementUserRepository> _repository;
    private readonly Mock<IMessagingQueues> _messagingQueues;
    private readonly ManagementUserServices _service;

    public UmsServiceTesting()
    {
        this._userService = new Mock<IUserService>();
        this._sessionService = new Mock<ISessionManagementServices>();
        this._jwtService = new Mock<IJwtServices>();
        this._redis = new Mock<IRedisRepository>();
        this._repository = new Mock<IManagementUserRepository>();
        this._messagingQueues = new Mock<IMessagingQueues>();

        this._service = new ManagementUserServices(
            this._userService.Object,
            this._sessionService.Object,
            this._jwtService.Object,
            this._redis.Object,
            this._messagingQueues.Object,
            this._repository.Object
        );
    }

    /// <summary>
    /// Update Any Credential
    /// </summary>
    [Fact]
    public async Task ShouldUpdateAnyCredential()
    {
        int id = 4;
        var user = UmsMocks.UserHashPassMock;
        var dto = UmsMocks.UpdateOwnRegisterDTOMock;

        var ms = new ManagementUserModel { UserId = id };

        string message = "Your reforms was saved successfully";

        this._userService.Setup(us => us.GetUserById(id)).ReturnsAsync(user);

        this._repository.Setup(r => r.GetRelationManagementByUserId(id)).ReturnsAsync(ms);
        this._repository.Setup(r => r.UpdateManagementUser(ms)).Returns(Task.CompletedTask);

        this._userService.Setup(us => us.UpdateOwnRegister(id, dto)).ReturnsAsync(message);

        var res = await this._service.UpdateAnyCrendetial(id, dto);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

    /// <summary>
    /// Should Update Email Throw Token
    /// </summary>
    [Fact]
    public async Task ShouldUpdateEmailThrowToken()
    {
        string ott = UmsMocks.TokensOttMock.NewEmailOtt;
        var user = UmsMocks.UserMock;

        int sessionId = 1;
        string newEmail = "rez@gmail.com";

        var claim = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("sessionId", sessionId.ToString()),
            new("new_email", newEmail)
        };

        string message = $"Hi {user.FullName}, your new email address was updated";

        this._redis.Setup(r => r.GetByTokenAsync(ott)).ReturnsAsync(ott);

        this._jwtService.Setup(j => j.ValidateOTT(ott)).ReturnsAsync
            (new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: claim)
            );

        this._jwtService.Setup(j => j.GetValuesFromClaims(It.IsAny<IEnumerable<Claim>>(), "sub")).Returns(user.Id.ToString());
        this._jwtService.Setup(j => j.GetValuesFromClaims(It.IsAny<IEnumerable<Claim>>(), "sessionId")).Returns(sessionId.ToString());
        this._jwtService.Setup(j => j.GetValuesFromClaims(It.IsAny<IEnumerable<Claim>>(), "new_email")).Returns(newEmail);

        this._userService.Setup(us => us.UpdateEmailAddress(user.Id, newEmail)).ReturnsAsync(user);
        this._sessionService.Setup(ss => ss.RemoveAllSessionExceptCurrent(user.Id, sessionId)).Returns(Task.CompletedTask);

        this._redis.Setup(rd => rd.UpdateStateAsync(ott)).ReturnsAsync(true);

        var res = await this._service.UpdateEmailAdress(ott);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

    /// <summary>
    /// Should Init A Request When The User Forget The Password
    /// </summary>
    [Fact]
    public async Task ShouldInitARequestWhenTheUserForgetThePassword()
    {
        var dto = UmsMocks.ForgetPasswordDTOMock;
        var user = UmsMocks.UserMock;
        var ott = UmsMocks.TokensOttMock.RecuperationToken;

        string message = "Check your email to continue with the operation";

        this._userService.Setup(us => us.GetUserByEmail(dto.Email)).ReturnsAsync(user);
        this._jwtService.Setup(jwt => jwt.GenerateRecuperationPasswordOTT(user)).ReturnsAsync(ott);
        this._messagingQueues.Setup(mq => mq.PasswordRecuperationMessage(user.Email, ott, user.Id)).Returns(Task.CompletedTask);

        var res = await this._service.ForgetPasswordAccount(dto);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

    /// <summary>
    /// Should Reset Password With A Ott
    /// </summary>
    [Fact]
    public async Task ShouldResetPasswordWithAOtt()
    {
        var user = UmsMocks.UserMock;
        var ott = UmsMocks.TokensOttMock.RecuperationToken;
        var dto = UmsMocks.PasswordDTOMock;

        var claim = new List<Claim> { new("sub", user.Id.ToString()) };

        string message = $"{user.FullName} your new password was chanches successfully";

        this._redis.Setup(r => r.GetByTokenAsync(ott)).ReturnsAsync(ott);

        this._jwtService.Setup(j => j.ValidateOTT(ott)).ReturnsAsync
            (new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: claim)
            );

        this._jwtService.Setup(jwt => jwt.GetValuesFromClaims(It.IsAny<IEnumerable<Claim>>(), "sub")).Returns(user.Id.ToString());

        this._userService.Setup(us => us.ReturnPassword(user.Id, dto)).ReturnsAsync(user);
        this._sessionService.Setup(ss => ss.RemoveAllSessionsByUserId(user.Id)).Returns(Task.CompletedTask);

        this._redis.Setup(rd => rd.UpdateStateAsync(ott)).ReturnsAsync(true);

        var res = await this._service.ResetPassword(ott, dto);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

}
