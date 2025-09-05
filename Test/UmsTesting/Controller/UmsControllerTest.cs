using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UmsTesting.Mock;
using UserManagementService.Modules.Controller;
using UserManagementService.Modules.Services.Interfaces;
using UserManagementService.Server.Users.Model;
using UserManagementService.Server.Users.Service.Interfaces;

namespace UmsTesting.Controller;

public class UmsControllerTest
{
    private readonly new Mock<IUserService> _userService;
    private readonly new Mock<IManagementUserServices> _managementService;
    private readonly UMSController _controller;

    public UmsControllerTest()
    {
        this._userService = new Mock<IUserService>();
        this._managementService = new Mock<IManagementUserServices>();

        this._controller = new UMSController(this._userService.Object, this._managementService.Object);
    }

    /// <summary>
    /// Should Get A List Of Users And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldGetAListOfUsersAndReturn200()
    {
        var list = new List<UserDTO> { UmsMocks.UserMockDTO };

        this._userService.Setup(us => us.ListOfAllUsers()).ReturnsAsync(list);

        var res = await this._controller.GetAllListOfUser();
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);

        Assert.Equal(list, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Get A User By Id And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldGetAUserByIdAndReturn200()
    {
        var user = UmsMocks.UserMockDTO;

        this._userService.Setup(us => us.GetUserProfile(user.Id)).ReturnsAsync(user);

        var res = await this._controller.GetUserById(user.Id);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);

        Assert.Equal(user, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Edit Own User Register And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldEditOwnUserRegisterAndReturn200()
    {
        int id = 4;
        var dto = UmsMocks.UpdateOwnRegisterDTOMock;

        string message = "Your reforms was saved successfully";

        this._managementService.Setup(ms => ms.UpdateAnyCrendetial(id, dto)).ReturnsAsync(message);

        var res = await this._controller.EditOwnAccount(id, dto);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);

        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Update Own User Roles And Return a 200 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateUserRolesAndReturn200()
    {
        int id = 4;
        var dto = UmsMocks.RolesDTOMock;

        string message = "Roles were updated successfully";

        this._userService.Setup(us => us.UpdateUserRoles(id, dto)).ReturnsAsync(message);

        var res = await this._controller.UpdateRoles(id, dto);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);

        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Delete Any User And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldDeleteAnyUserAndReturn200()
    {
        int id = 4;
        var user = UmsMocks.UserMock;
        string message = $"User {user.FullName} was deleted successfully";

        this._userService.Setup(us => us.RemoveAnyAccount(id)).ReturnsAsync(message);

        var res = await this._controller.DeleteAnyAccount(id);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);

        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Validate New Email And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldValidateNewEmailAndReturn200()
    {
        var klt1276 = UmsMocks.TokensOttMock.NewEmailOtt;
        var user = UmsMocks.UserMock;

        string message = $"Hi {user.FullName}, your new email address was updated";

        this._managementService.Setup(ms => ms.UpdateEmailAdress(klt1276)).ReturnsAsync(message);

        var res = await this._controller.ValidateNewEmail(klt1276);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);

        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Create A Request To Return Account And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldCreateARequestToReturnAccountAndReturn200()
    {
        var dto = UmsMocks.ForgetPasswordDTOMock;

        string message = "Check your email to continue with the operation";

        this._managementService.Setup(ms => ms.ForgetPasswordAccount(dto)).ReturnsAsync(message);

        var res = await this._controller.ForgetPassword(dto);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);

        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Should Restart Password And Return 200
    /// </summary>
    [Fact]
    public async Task ShouldRestartPasswordAndReturn200()
    {
        string hmk12 = UmsMocks.TokensOttMock.RecuperationToken;
        var dto = UmsMocks.PasswordDTOMock;
        var user = UmsMocks.UserMock;

        string message = $"{user.FullName} your new password was chanches successfully";

        this._managementService.Setup(ms => ms.ResetPassword(hmk12, dto)).ReturnsAsync(message);

        var res = await this._controller.ReturningPassword(hmk12, dto);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);

        Assert.Equal(message, result.Value);
        Assert.Equal(200, result.StatusCode);
    }

}
