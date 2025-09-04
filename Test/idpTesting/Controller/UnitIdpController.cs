using System.Threading.Tasks;
using IdentifyService.Module.Controller;
using IdentifyService.Module.Services.Interfaces;
using IdentifyService.Server.UMS.Model;
using idpTesting.Mock;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace idpTesting.Controller;

public class UnitIdpController
{
    private readonly Mock<IIdentityProviderService> _service;
    private readonly IdpController _controller;

    public UnitIdpController()
    {
        this._service = new Mock<IIdentityProviderService>();
        this._controller = new IdpController(this._service.Object);
    }

    /// <summary>
    /// Respone a 200ok  when user was registered
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldGiveA200OkWhenUserWasRegistered()
    {
        var body = IdentityServiceMock.CreateUserDTOMOck;
        var user = IdentityServiceMock.UserMock;
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
        var body = IdentityServiceMock.LoginDTOMock;
        var user = IdentityServiceMock.UserMock;

        var token = IdentityServiceMock.TokenMock.AccessToken;

        var httpContext = new DefaultHttpContext();
        httpContext.Items["User"] = user;

        this._controller.ControllerContext.HttpContext = httpContext;
        this._service.Setup(s => s.Login(user)).ReturnsAsync(token);

        var result = await this._controller.Login(body) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Init Session
    /// </summary>
    [Fact]
    public async Task ShouldInitSessionReturn200()
    {
        var dto = IdentityServiceMock.VerifyCodeDTOMock;
        var user = IdentityServiceMock.UserMock;
        string message = $"Welcome {user.FullName}";

        this._service.Setup(s => s.InitSession(dto)).ReturnsAsync(message);

        var res = await this._controller.StartSession(dto);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(message, result.Value);
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
    /// Should Get All List Of User Session Return 200
    /// </summary>
    [Fact]
    public async Task ShouldGetAllListOfUserSessionReturn200()
    {
        var list = new List<SessionModel> { IdentityServiceMock.SessionModelMock };

        this._service.Setup(s => s.ListOfAllSessionsAsync()).ReturnsAsync(list);

        var res = await this._controller.GetAllSessions();
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(list, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Delte A Session By Id And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldDelteASessionByIdAndReturn200()
    {
        int id = 2;
        string message = "This session was deleted successfully";

        this._service.Setup(s => s.RemoveOneSessionById(id)).ReturnsAsync(message);

        var res = await this._controller.DeleteSessionById(id);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Verify RBA And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldVerifyRBAAndReturn200()
    {
        string k892 = IdentityServiceMock.TokenVerify.VerifySessionOTT;
        string message = "Your new session was saved successfully, now you cann init session";

        this._service.Setup(s => s.VerifySession(k892)).ReturnsAsync(message);

        var res = await this._controller.RBAVerify(k892);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Refresh Token
    /// </summary>
    [Fact]
    public async Task RefreshToken()
    {
        var token = IdentityServiceMock.TokenMock.AccessToken;
        this._service.Setup(s => s.GenerateRefreshToken()).ReturnsAsync(token);

        var res = await this._controller.RefreshToken() as NoContentResult;

        Assert.NotNull(res);
        Assert.Equal(204, res.StatusCode);
    }

    /// <summary>
    /// Should return a 200 ok in Verify Email 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturn200OkinVerifyEmail()
    {
        string kl124 = IdentityServiceMock.TokenVerify.VerifyEmailOTT;
        var user = IdentityServiceMock.UserMock;
        string message = $"Hello {user.FullName} your account was verificate successfully.";

        this._service.Setup(s => s.VerificationEmail(kl124)).ReturnsAsync(message);

        var res = await this._controller.VerifyEmail(kl124);
        var respons = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(respons);
        Assert.Equal(200, respons.StatusCode);
        Assert.Equal(message, respons.Value);
    }

    /// <summary>
    /// Should Generate 2AF Code And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldGenerate2AFCodeAndReturn200()
    {
        string message = $"Check your email to singing code";

        this._service.Setup(s => s.TwoFactorAuthenticationCodeGeneration()).ReturnsAsync(message);

        var res = await this._controller.GenerateTwoFACode();
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Remove Own Account 200Ok 
    /// </summary>
    [Fact]
    public async Task Return200OkRemoveOwnAccountAsync()
    {
        string message = "Your account will be deleted in the next 10 minutes.";
        var body = IdentityServiceMock.RemoveOwnAccountDTOMock;

        this._service.Setup(s => s.RemoveOwnAccount(body)).ReturnsAsync(message);

        var res = await this._controller.RemoveOwnAccountAsync(body);
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
        var body = IdentityServiceMock.NewEmailMock;

        var user = IdentityServiceMock.UserMock;
        string message = $"Email was updated his new email is {user.Email} ";

        this._service.Setup(s => s.ChangeAddressEmail(body)).ReturnsAsync(message);

        var res = await this._controller.ReformEmailAddres(body);
        var response = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(message, response.Value);
    }

    /// <summary>
    /// Return 200 And Reform Password
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Return200AndReformPassword()
    {
        var dto = IdentityServiceMock.UpdatePasswordDTOMock;
        string message = "Password updated successfully";

        this._service.Setup(s => s.ChangePassword(dto)).ReturnsAsync(message);

        var res = await this._controller.UpdatePassword(dto);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

}
