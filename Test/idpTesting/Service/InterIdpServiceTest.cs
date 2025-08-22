using System.Globalization;
using System.Security.Claims;
using IdentifyService._2FA.Interfaces;
using IdentifyService.Configuration.Redis.Repository.Interfaces;
using IdentifyService.Cookies.Interfaces;
using IdentifyService.JWT.Interfaces;
using IdentifyService.Module.Model;
using IdentifyService.Module.Repository.Interface;
using IdentifyService.Module.Services;
using IdentifyService.Queues.Messaging.Interfaces;
using IdentifyService.Server.UMS.Services.Interfaces;
using IdentifyService.Utils.Helper;
using idpTesting.Mock;
using Microsoft.AspNetCore.Http;
using Moq;

namespace idpTesting.Service;

public class InterIdpServiceTest
{
    private readonly Mock<IIdentityProviderRepository> _repository;
    private readonly Mock<IHttpContextAccessor> _httpContext;
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<ICookieService> _cookieService;
    private readonly Mock<IMessagingQueues> _messagingQueues;
    private readonly Mock<IRedisRepository> _redisRepository;
    private readonly Mock<CodeGeneration> _codeGeneration;
    private readonly Mock<IValidateTwoFactorAuth> _validateTwoFactor;
    private readonly Mock<IManagementUserFacedeServices> _managementUser;
    private readonly IdentityProviderService _service;

    public InterIdpServiceTest()
    {
        this._repository = new Mock<IIdentityProviderRepository>();
        this._httpContext = new Mock<IHttpContextAccessor>();
        this._jwtService = new Mock<IJwtService>();
        this._cookieService = new Mock<ICookieService>();
        this._messagingQueues = new Mock<IMessagingQueues>();
        this._redisRepository = new Mock<IRedisRepository>();
        this._codeGeneration = new Mock<CodeGeneration>();
        this._validateTwoFactor = new Mock<IValidateTwoFactorAuth>();
        this._managementUser = new Mock<IManagementUserFacedeServices>();


        this._httpContext.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

        this._service = new IdentityProviderService(
            this._httpContext.Object,
            this._repository.Object,
            this._jwtService.Object,
            this._cookieService.Object,
            this._messagingQueues.Object,
            this._redisRepository.Object,
            this._codeGeneration.Object,
            this._validateTwoFactor.Object,
            this._managementUser.Object
        );
    }
    /// <summary>
    /// Register User
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldRegisterAUser()
    {
        var body = IdentityServiceMock.CreateUserDTOMOck;
        var user = IdentityServiceMock.UserMock;
        var token = IdentityServiceMock.TokenVerify;

        string message = $"Your was registerd successfully, now you need check your email to verificated";

        this._managementUser.Setup(m => m.SaveUserRegistered(body)).ReturnsAsync(user);
        this._repository.Setup(r => r.SaveAuth(user.Id)).Returns(Task.CompletedTask);

        this._jwtService.Setup(j => j.GenerateEmailVerificationOTT(user)).ReturnsAsync(token.VerifyEmail);
        this._messagingQueues.Setup(m => m.SendEmailVerificactionEvent(user.Email, token.VerifyEmail, user.Id)).Returns(Task.CompletedTask);

        var res = await this._service.RegisteredUser(body);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

    /// <summary>
    /// Refresh Token
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldGenerateRefreshToken()
    {
        var user = IdentityServiceMock.UserMock;
        int sessionId = 1;
        var token = IdentityServiceMock.TokenMock;

        var idp = new AuthModel { Id = 1, UserId = user.Id };

        var claim = new List<Claim> { new("sub", user.Id.ToString()) };

        this._jwtService.Setup(j => j.GetClaimFromToken()).Returns(claim);
        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sub")).Returns(user.Id.ToString);
        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sessionId")).Returns(sessionId.ToString);

        this._managementUser.Setup(u => u.FindUserById(user.Id)).ReturnsAsync(user);
        this._jwtService.Setup(j => j.GenerateRefreshToken(sessionId, user)).Returns(token);

        this._repository.Setup(r => r.FindAuthByUserId(user.Id)).ReturnsAsync(idp);
        this._repository.Setup(r => r.UpdateAsync(idp)).ReturnsAsync(true);

        var result = await this._service.GenerateRefreshToken();

        this._cookieService.Verify(x => x.SetTokenCookies(It.IsAny<HttpResponse>(), token), Times.Once);

        Assert.Equal(token.AccessToken, result);

    }

    /// <summary>
    /// Login
    /// </summary>
    [Fact]
    public async Task ShouldLoginUser()
    {
        var user = IdentityServiceMock.UserMock;
        var token = IdentityServiceMock.TokenMock;

        var idp = new AuthModel { Id = 1, UserId = user.Id };
        int sessionId = 1;

        string message = $"Check your email to singing code";

        this._repository.Setup(r => r.FindAuthByUserId(user.Id)).ReturnsAsync(idp);

        this._jwtService.Setup(j => j.GenerateAuthenticationToken(sessionId, user)).Returns(token);
        this._repository.Setup(r => r.UpdateAsync(idp)).ReturnsAsync(true);

        var result = await this._service.Login(user);

        this._cookieService.Verify(x => x.SetTokenCookies(It.IsAny<HttpResponse>(), token), Times.Once);

        Assert.Equal(message, result);
    }

    /// <summary>
    /// Get Value by Cookie
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturnOneValueOfCookie()
    {
        var user = IdentityServiceMock.UserMock;
        int sessionId = 1;
        var claim = new List<Claim> { new("sub", user.Id.ToString()) };

        this._jwtService.Setup(j => j.GetClaimFromToken()).Returns(claim);
        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sub")).Returns(user.Id.ToString);
        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sessionId")).Returns(sessionId.ToString);

        this._managementUser.Setup(u => u.FindUserById(user.Id)).ReturnsAsync(user);

        var result = await this._service.GetValueByCookie();

        Assert.NotNull(result);
        Assert.Equal(user.Username, result.User.Username);
    }

    /// <summary>
    /// Log Out
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldClouseSectionLogOut()
    {
        var user = IdentityServiceMock.UserMock;

        var claim = new List<Claim> { new("sub", user.Id.ToString()) };

        var idp = new AuthModel { Id = 1, UserId = user.Id };

        this._jwtService.Setup(j => j.GetClaimFromToken()).Returns(claim);
        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sub")).Returns(user.Id.ToString);

        this._repository.Setup(r => r.FindAuthByUserId(user.Id)).ReturnsAsync(idp);
        this._repository.Setup(r => r.UpdateAsync(idp)).ReturnsAsync(true);

        var res = this._service.LogOut();

        Assert.NotNull(res);
    }

    /// <summary>
    /// Should Solicited Delation Account
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldSolicitedDelationAccount()
    {
        int id = 4;
        var body = IdentityServiceMock.RemoveOwnAccountDTOMock;
        var user = IdentityServiceMock.UserHashPassMock;

        var claim = new List<Claim> { new("sub", user.Id.ToString()) };

        var idp = new AuthModel { Id = 1, UserId = user.Id };

        string message = "Your account will be deleted in the next 10 minutes.";

        this._jwtService.Setup(j => j.GetClaimFromToken()).Returns(claim);
        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sub")).Returns(user.Id.ToString);

        this._validateTwoFactor.Setup(v => v.ImplementValidate(id, body.Code)).ReturnsAsync(idp);

        this._managementUser.Setup(u => u.FindUserById(id)).ReturnsAsync(user);
        this._managementUser.Setup(u => u.RequestToRemoveOwnAccount(id)).Returns(Task.CompletedTask);

        this._repository.Setup(r => r.UpdateAsync(idp)).ReturnsAsync(true);

        var res = await this._service.RemoveOwnAccount(body);

        Assert.Equal(message, res);
    }
    /// <summary>
    /// Refresh Token Validation
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateRefreshToken()
    {
        var user = IdentityServiceMock.UserMock;
        var token = IdentityServiceMock.TokenMock;

        var idp = new AuthModel { Id = 1, UserId = user.Id };

        this._repository.Setup(r => r.FindAuthByUserId(user.Id)).ReturnsAsync(idp);

        var res = this._service.RefreshTokenValidate(token.RefreshHasherToken, user.Id);

        Assert.NotNull(res);
    }

    /// <summary>
    /// Should Validate Email
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateEmail()
    {
        var token = IdentityServiceMock.TokenVerify.PasswordRecuperation;
        var user = IdentityServiceMock.UserMock;
        int id = 4;

        var claim = new List<Claim> { new("sub", id.ToString()) };

        var idp = new AuthModel { Id = 1, UserId = user.Id };

        string message = $"Hello {user.FullName} your account was verificate successfully.";

        this._redisRepository.Setup(r => r.GetByTokenAsync(token)).ReturnsAsync(token);

        this._jwtService.Setup(j => j.ValidateOTT(token)).ReturnsAsync
            (new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: [new Claim("sub", id.ToString())])
            );

        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sub")).Returns(id.ToString);

        this._managementUser.Setup(u => u.FindUserById(id)).ReturnsAsync(user);

        this._repository.Setup(u => u.FindAuthByUserId(id)).ReturnsAsync(idp);
        this._repository.Setup(u => u.UpdateAsync(idp)).ReturnsAsync(true);

        this._messagingQueues
            .Setup(m => m.SendWelcomeMessage(user.FullName, user.Email, user.Id))
            .Returns(Task.CompletedTask);

        this._redisRepository.Setup(r => r.UpdateStateAsync(token)).ReturnsAsync(true);

        var res = await this._service.VerificationEmail(token);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

    /// <summary>
    /// Should Update Email 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateNewEmail()
    {
        int id = 4;
        int sessionId = 1;

        var body = IdentityServiceMock.NewEmailMock;
        var user = IdentityServiceMock.UserMock;

        var claim = new List<Claim> { new("sub", user.Id.ToString()) };

        var idp = new AuthModel { Id = 1, UserId = user.Id };

        string message = $"Email was updated his new email is {user.Email} ";

        this._jwtService.Setup(j => j.GetClaimFromToken()).Returns(claim);
        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sub")).Returns(user.Id.ToString);
        this._jwtService.Setup(j => j.GetValuesFromClaims(claim, "sessionId")).Returns(sessionId.ToString);

        this._repository.Setup(r => r.FindAuthByUserId(id)).ReturnsAsync(idp);

        this._managementUser.Setup(u => u.VerifyNewEmailParameters(id, body)).ReturnsAsync(user);

        this._jwtService.Setup(j => j.GenerateVerifyNewEmailOTT(user, sessionId, body.NewEmail));

        this._messagingQueues
            .Setup(m => m.SendNewEmailVerificationEvent(user.FullName, user.Email, user.Id))
            .Returns(Task.CompletedTask);

        this._repository.Setup(r => r.UpdateAsync(idp)).ReturnsAsync(true);

        string res = await this._service.ChangeAddressEmail(body);

        Assert.Equal(message, res);
    }

    /// <summary>
    /// Validate Credential
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateUserCredentials()
    {
        var body = IdentityServiceMock.LoginDTOMock;
        var user = IdentityServiceMock.UserMockEmailTrue;

        var idp = new AuthModel { Id = 1, UserId = user.Id , EmailVerify = true};

        this._managementUser.Setup(u => u.FindByValue(body.Username)).ReturnsAsync(user);

        this._repository.Setup(r => r.FindAuthByUserId(user.Id)).ReturnsAsync(idp);
        this._repository.Setup(r => r.UpdateAsync(idp)).ReturnsAsync(true);

        this._managementUser.Setup(m => m.CancelRemoveAccountOperationIfOn(user.Id)).Returns(Task.CompletedTask);

        var res = this._service.ValidateUserCredential(body);

        Assert.NotNull(res);
        Assert.Equal(user, res.Result);
    }

}
