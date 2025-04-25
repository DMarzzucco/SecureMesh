using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Security.Module.Controller;
using Security.Module.Services.Interfaces;
using SecurityTesting.Mock;

namespace SecurityTesting;

public class UnitSecurityTest
{
    private readonly Mock<ISecurityService> _service;
    private readonly SecurityController _controller;

    public UnitSecurityTest()
    {
        this._service = new Mock<ISecurityService>();
        this._controller = new SecurityController(this._service.Object);
    }

    /// <summary>
    /// login user
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldLoginTheUser()
    {
        var body = SecurityMock.LoginDTOMock;
        var user = SecurityMock.UserMock;

        var token = SecurityMock.TokenMock.AccessToken;

        var httpContext = new DefaultHttpContext();
        httpContext.Items["User"] = user;

        this._controller.ControllerContext.HttpContext = httpContext;
        this._service.Setup(s => s.GenerateToken(user)).ReturnsAsync(token);

        var result = await this._controller.Login(body) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Get Profile 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShoudlGetProfile()
    {
        var user = SecurityMock.UserMock;
        this._service.Setup(s => s.GetProfile()).ReturnsAsync(user);

        var result = await this._controller.GetProfile();
        var res = Assert.IsType<OkObjectResult>(result.Result);

        Assert.NotNull(res);
        Assert.Equal(200, res.StatusCode);
        Assert.Equal(user, res.Value);
    }

    /// <summary>
    /// Log Out
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldCloseSection()
    {
        this._service.Setup(s => s.LogOut()).Returns(Task.CompletedTask);

        var res = await this._controller.CloseSection() as OkObjectResult;

        Assert.NotNull(res);
        Assert.Equal(200, res.StatusCode);
    }

    /// <summary>
    /// Should Refresh Token
    /// </summary>
    [Fact]
    public async Task RefreshToken()
    {
        var token = SecurityMock.TokenMock.AccessToken;
        this._service.Setup(s => s.GenerateRefreshToken()).ReturnsAsync(token);

        var res = await this._controller.RefreshToken() as NoContentResult;

        Assert.NotNull(res);
        Assert.Equal(204, res.StatusCode);
    }
}
