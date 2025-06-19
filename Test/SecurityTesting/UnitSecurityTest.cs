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
    /// Respone a 200ok  when user was registered
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldGiveA200OkWhenUserWasRegistered()
    {
        var body = SecurityMock.CreateUserDTOMOck;
        var user = SecurityMock.UserMock;
        string message = $"Your was registerd successfully, now you need check your email to verificated";

        this._service.Setup(s => s.RegisteredUser(body)).ReturnsAsync(message);

        var res = await this._controller.Registered(body);
        var respons = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(respons);
        Assert.Equal(200, respons.StatusCode);
        Assert.Equal(message, respons.Value);
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

    /// <summary>
    /// Must return a 200ok in Forgett password
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Should200OkForgettPassword()
    {
        var body = SecurityMock.FotgetPasswordMock;
        var user = SecurityMock.UserMock;
        string message = "You need check your email to next.";

        this._service.Setup(s => s.ForgetPassword(body)).ReturnsAsync(message);

        var res = await this._controller.ForgetPassword(body);
        var response = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(message, response.Value);
    }

    /// <summary>
    /// Should return a 200 ok in Verify Email 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturn200OkinVerifyEmail()
    {
        string kl124 = SecurityMock.TokenVerify.VerifyEmail;
        var user = SecurityMock.UserMock;
        string message = $"Hello {user.FullName} your account was verificate successfully.";

        this._service.Setup(s => s.VerificationEmail(kl124)).ReturnsAsync(message);

        var res = await this._controller.VerifyEmail(kl124);
        var respons = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(respons);
        Assert.Equal(200, respons.StatusCode);
        Assert.Equal(message, respons.Value);
    }
    /// <summary>
    /// 200 Ok  verify new email 
    /// </summary>
    [Fact]
    public async Task Return200OkVerifyNewEmailAsync()
    {
        string klt1276 = SecurityMock.TokenVerify.VerifyEmail;
        var user = SecurityMock.UserMock;
        string message = $"{user.Username} your new adress was verificate successfully, now you can login in.";

        this._service.Setup(s => s.VerificationNewEmail(klt1276)).ReturnsAsync(message);

        var res = await this._controller.VerifyNewEmail(klt1276);
        var respons = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(respons);
        Assert.Equal(200, respons.StatusCode);
        Assert.Equal(message, respons.Value);
    }
    /// <summary>
    /// Remove Own Account 200Ok 
    /// </summary>
    [Fact]
    public async Task Return200OkRemoveOwnAccountAsync()
    {
        int id = 4;
        string message = "Your account will be deleted in the next 10 minutes.";
        var body = SecurityMock.PasswordDTOMock;

        this._service.Setup(s => s.RemoveOwnAccount(id, body)).ReturnsAsync(message);

        var res = await this._controller.RemoveOwnAccountAsync(id, body);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(message, result.Value);
    }

    /// <summary>
    /// Return 200 in Reform Email Address
    /// </summary>
    [Fact]
    public async Task Return200OkReformEmailAddress()
    {
        int id = 4;
        var body = SecurityMock.NewEmailMOck;

        var user = SecurityMock.UserMock;
        string message = $"Email was updated his new email is {user.Email} ";

        this._service.Setup(s => s.ChangeAddressEmail(id, body)).ReturnsAsync(message);

        var res = await this._controller.ReformEmailAddres(id, body);
        var response = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(message, response.Value);
    }
    /// <summary>
    /// Retunr 200 Ok in Returning Password
    /// </summary>
    [Fact]
    public async Task Return200OkReturningPassword()
    {
        string hmk12 = SecurityMock.TokenVerify.PasswordRecuperation;
        var body = SecurityMock.NewPasswordDTOMock;

        var user = SecurityMock.UserMock;
        string message = $"{user.FullName} Your new password was chanchis successfully";

        this._service.Setup(s => s.ResetPassword(hmk12, body)).ReturnsAsync(message);

        var res = await this._controller.ReturningPassword(hmk12, body);
        var response = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(message, response.Value);
    }
}
