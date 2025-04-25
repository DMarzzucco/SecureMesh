using Microsoft.AspNetCore.Http;
using Moq;
using Security.Cookies.Interfaces;
using Security.JWT.Interfaces;
using Security.Module.Services;
using Security.Server.Service.Interfaces;
using SecurityTesting.Mock;

namespace SecurityTesting;

public class InterSecurityTest
{
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<IHttpContextAccessor> _httpContext;
    private readonly Mock<ICookieService> _cookieService;
    private readonly Mock<IUserService> _userService;
    private readonly SecurityService _service;

    public InterSecurityTest()
    {
        this._jwtService = new Mock<IJwtService>();
        this._httpContext = new Mock<IHttpContextAccessor>();
        this._cookieService = new Mock<ICookieService>();
        this._userService = new Mock<IUserService>();

        this._httpContext.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        this._service = new SecurityService(
            this._httpContext.Object,
            this._jwtService.Object,
            this._cookieService.Object,
            this._userService.Object
        );
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

        this._jwtService.Setup(j => j.GenerateToken(user)).Returns(token);

        var result = await this._service.GenerateToken(user);

        this._userService.Verify(x => x.UpdateRefreshToken(user.Id, token.RefreshHasherToken), Times.Once);

        this._cookieService.Verify(x => x.SetTokenCookies(It.IsAny<HttpResponse>(), token), Times.Once);

        Assert.Equal(token.AccessToken, result);
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
        this._userService.Setup(u=> u.GetUserById(Id)).ReturnsAsync(user);

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

        this._userService.Setup(u=> u.UpdateRefreshToken(user.Id, null)).Returns(Task.CompletedTask);

        var res = this._service.LogOut();

        Assert.NotNull(res);
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
    /// Validate Credential
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateUserCredentials()
    {
        var body = SecurityMock.LoginDTOMock;
        var user = SecurityMock.UserHashPassMock;

        this._userService.Setup(u=> u.FindByValue("Username", body.Username)).ReturnsAsync(user);
        
        var res =  this._service.ValidateUser(body);

        Assert.NotNull(res);
        Assert.Equal(user, res.Result);
    }
}
