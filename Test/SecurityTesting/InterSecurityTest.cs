using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Security.Configuration.Redis.Repository.Interfaces;
using Security.Cookies.Interfaces;
using Security.JWT.Interfaces;
using Security.Module.Services;
using Security.Queues.Messaging.Interfaces;
using Security.Server.Service.Interfaces;
using SecurityTesting.Mock;

namespace SecurityTesting;

public class InterSecurityTest
{
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<IHttpContextAccessor> _httpContext;
    private readonly Mock<ICookieService> _cookieService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IMessagingQueues> _messagingQueues;
    private readonly Mock<IRedisRepository> _redisRepository;
    private readonly SecurityService _service;

    public InterSecurityTest()
    {
        this._jwtService = new Mock<IJwtService>();
        this._httpContext = new Mock<IHttpContextAccessor>();
        this._cookieService = new Mock<ICookieService>();
        this._userService = new Mock<IUserService>();
        this._redisRepository = new Mock<IRedisRepository>();
        this._messagingQueues = new Mock<IMessagingQueues>();

        this._httpContext.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        this._service = new SecurityService(
            this._httpContext.Object,
            this._jwtService.Object,
            this._cookieService.Object,
            this._userService.Object,
            this._messagingQueues.Object,
            this._redisRepository.Object
        );
    }
    /// <summary>
    /// Register User
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldRegisterAUser()
    {
        var body = SecurityMock.CreateUserDTOMOck;
        var user = SecurityMock.UserMock;
        var token = SecurityMock.TokenVerify;

        string message = $"Your was registerd successfully, now you need check your email to verificated";

        this._jwtService.Setup(j => j.GenerateEmailVerificationToken(user)).ReturnsAsync(token.VerifyEmail);
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
        var user = SecurityMock.UserMock;
        var token = SecurityMock.TokenMock;

        this._jwtService.Setup(j => j.GetIdFromToken()).Returns(user.Id);
        this._userService.Setup(u => u.GetUserById(user.Id)).ReturnsAsync(user);
        this._jwtService.Setup(j => j.RefreshToken(user)).Returns(token);

        var result = await this._service.GenerateRefreshToken();

        this._userService.Verify(x => x.UpdateRefreshToken(user.Id, token.RefreshHasherToken), Times.Once);
        this._cookieService.Verify(x => x.SetTokenCookies(It.IsAny<HttpResponse>(), token), Times.Once);

        Assert.Equal(token.AccessToken, result);

    }

    /// <summary>
    /// Generate Access Token
    /// </summary>
    [Fact]
    public async Task ShouldGenerateToken()
    {
        var user = SecurityMock.UserMock;
        var token = SecurityMock.TokenMock;
        string message = $"Welcome {user.FullName}";

        this._jwtService.Setup(j => j.GenerateToken(user)).Returns(token);

        var result = await this._service.GenerateToken(user);

        this._userService.Verify(x => x.UpdateRefreshToken(user.Id, token.RefreshHasherToken), Times.Once);

        this._cookieService.Verify(x => x.SetTokenCookies(It.IsAny<HttpResponse>(), token), Times.Once);

        Assert.Equal(message, result);
    }

    /// <summary>
    /// Get user by Cookie
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturnOneUserRegister()
    {
        int Id = 4;
        var user = SecurityMock.UserMock;

        this._jwtService.Setup(j => j.GetIdFromToken()).Returns(Id);
        this._userService.Setup(u => u.GetUserById(Id)).ReturnsAsync(user);

        var result = await this._service.GetUserByCookie();

        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }

    /// <summary>
    /// Log Out
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldClouseSectionLogOut()
    {
        var user = SecurityMock.UserMock;

        this._userService.Setup(u => u.UpdateRefreshToken(user.Id, null)).Returns(Task.CompletedTask);

        var res = this._service.LogOut();

        Assert.NotNull(res);
    }

    /// <summary>
    /// Should Send a Message Of Recuperation To RabbitMQ 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldSendAMessaginOfRecuperationToRabbitMQ()
    {
        var body = SecurityMock.FotgetPasswordMock;
        var token = SecurityMock.TokenVerify.PasswordRecuperation;
        var user = SecurityMock.UserMock;
        string message = "You need check your email to next.";

        this._userService.Setup(u => u.GetUserByEmail(body.Email)).ReturnsAsync(user);
        this._jwtService.Setup(j => j.GenerateRecuperationPasswordToken(user)).ReturnsAsync(token);
        this._messagingQueues.Setup(m => m.PasswordRecuperationMessage(user.Email, token, user.Id)).Returns(Task.CompletedTask);

        var res = await this._service.ForgetPassword(body);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }
    /// <summary>
    /// Should Solicited Delation Account
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldSolicitedDelationAccount()
    {
        int id = 4;
        var body = SecurityMock.PasswordDTOMock;
        var user = SecurityMock.UserHashPassMock;
        string message = "Your account will be deleted in the next 10 minutes.";

        this._userService.Setup(u => u.DeletedOwnAccount(id, body)).ReturnsAsync(message);
        this._userService.Setup(u => u.GetUserById(id)).ReturnsAsync(user);
        this._userService.Setup(u => u.UpdateRefreshToken(user.Id, null)).Returns(Task.CompletedTask);

        var res = await this._service.RemoveOwnAccount(id, body);

        Assert.Equal(message, res);
    }
    /// <summary>
    /// Refresh Token Validation
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateRefreshToken()
    {
        var user = SecurityMock.UserMock;
        int id = 4;
        var token = SecurityMock.TokenMock;

        this._userService.Setup(u => u.GetUserById(id)).ReturnsAsync(user);

        var res = await this._service.RefreshTokenValidate(user.RefreshToken, id);

        Assert.Equal(user, res);
    }

    /// <summary>
    /// Should Reseet Password
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldResetPassword()
    {
        var token = SecurityMock.TokenVerify.PasswordRecuperation;
        var body = SecurityMock.NewPasswordDTOMock;
        var user = SecurityMock.UserMock;

        int id = 4;
        string message = $"{user.FullName} Your new password was chanchis successfully";

        this._redisRepository.Setup(r => r.GetByTokenAsync(token)).ReturnsAsync(token);
        this._jwtService.Setup(j => j.ValidateVerificationToken(token)).ReturnsAsync
            (new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: [new Claim("sub", id.ToString())])
            );

        this._userService.Setup(u => u.ReturnPassword(id, body)).ReturnsAsync(user);
        this._redisRepository.Setup(r => r.UpdateStateAsync(token)).ReturnsAsync(true);

        var res = await this._service.ResetPassword(token, body);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

    /// <summary>
    /// Should Validate Email
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateEmail()
    {
        var token = SecurityMock.TokenVerify.PasswordRecuperation;
        var user = SecurityMock.UserMock;
        int id = 4;
        string message = $"Hello {user.FullName} your account was verificate successfully.";

        this._redisRepository.Setup(r => r.GetByTokenAsync(token)).ReturnsAsync(token);
        this._jwtService.Setup(j => j.ValidateVerificationToken(token)).ReturnsAsync
            (new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: [new Claim("sub", id.ToString())])
            );

        this._userService.Setup(u => u.GetUserById(id)).ReturnsAsync(user);
        this._userService.Setup(u => u.MarkEmailAsync(id)).ReturnsAsync(user);
        this._messagingQueues
            .Setup(m => m.SendWelcomeMessage(user.FullName, user.Email, user.Id))
            .Returns(Task.CompletedTask);

        this._redisRepository.Setup(r => r.UpdateStateAsync(token)).ReturnsAsync(true);

        var res = await this._service.VerificationEmail(token);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

    /// <summary>
    /// Should Validate a New Email
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateNewEmailAsync()
    {
        var token = SecurityMock.TokenVerify.PasswordRecuperation;
        var user = SecurityMock.UserMock;
        int id = 4;
        string message = $"{user.Username} your new adress was verificate successfully, now you can login in.";

        this._redisRepository.Setup(r => r.GetByTokenAsync(token)).ReturnsAsync(token);
        this._jwtService.Setup(j => j.ValidateVerificationToken(token)).ReturnsAsync
            (new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: [new Claim("sub", id.ToString())])
            );
        this._userService.Setup(u => u.GetUserById(id)).ReturnsAsync(user);
        this._userService.Setup(u => u.MarkEmailAsync(id)).ReturnsAsync(user);
        this._redisRepository.Setup(r => r.UpdateStateAsync(token)).ReturnsAsync(true);

        var res = await this._service.VerificationNewEmail(token);

        Assert.NotNull(res);
        Assert.Equal(message, res);

    }

    /// <summary>
    /// Validate Credential
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateUserCredentials()
    {
        var body = SecurityMock.LoginDTOMock;
        var user = SecurityMock.UserMockEmailTrue;

        this._userService.Setup(u => u.FindByValue("Username", body.Username)).ReturnsAsync(user);

        var res = this._service.ValidateUser(body);

        Assert.NotNull(res);
        Assert.Equal(user, res.Result);
    }

    /// <summary>
    /// Should Update Email 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateNewEmail()
    {
        int id = 4;
        var body = SecurityMock.NewEmailMOck;
        var user = SecurityMock.UserMock;
        string message = $"Email was updated his new email is {user.Email} ";

        this._userService.Setup(u => u.UpdateEmailAdress(id, body)).ReturnsAsync(user);
        this._userService.Setup(u => u.UpdateRefreshToken(id, null)).Returns(Task.CompletedTask);

        var res = await this._service.ChangeAddressEmail(id, body);

        Assert.Equal(message, res);
    }
}


