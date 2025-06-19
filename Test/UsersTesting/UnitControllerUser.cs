using Microsoft.AspNetCore.Mvc;
using Moq;
using User.Module.Controller;
using User.Module.DTOs;
using User.Module.Service.Interface;
using UsersTesting.Mock;

namespace UsersTesting;

public class UnitControllerUser
{
    private readonly Mock<IUserService> _service;
    private readonly UserController _controller;

    public UnitControllerUser()
    {
        this._service = new Mock<IUserService>();
        this._controller = new UserController(this._service.Object);
    }

    /// <summary>
    /// Get All Users
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturnAListOfAllRegister()
    {
        var list = new List<UserDTO> { UsersMock.UserMockDTO };

        this._service.Setup(s => s.ListOfAllRegister()).ReturnsAsync(list);
        var res = await this._controller.GetAllRegister();
        var response = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(list, response.Value);
    }

    /// <summary>
    /// Return User By Id
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturnAUserByKey()
    {
        var id = 4;
        var user = UsersMock.UserMockDTO;

        this._service.Setup(s => s.GetUserProfileById(id)).ReturnsAsync(user);
        var res = await this._controller.GetUserById(id);
        var response = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(user, response.Value);
    }
    /// <summary>
    /// Edit User
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldEditUserRegister()
    {
        var body = UsersMock.UpdateUserDTOMOck;
        var id = 5;
        var user = UsersMock.UserMockBasic;

        this._service.Setup(s => s.UpdateRegister(body, id)).ReturnsAsync(user);
        var res = await this._controller.EditUser(id, body) as NoContentResult;

        Assert.NotNull(res);
        Assert.Equal(204, res.StatusCode);
    }

    /// <summary>
    /// Edit Own Register
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task  ShouldUPdateOwnRegister()
    {
        int id = 4;
        var body = UsersMock.UpdateOwnUserDTOMock;

        string message = "Your reforms was saved successfully";

        this._service.Setup(s=> s.UpdateOwnRegister(id,body)).ReturnsAsync(message);

        var res = await this._controller.EditOwnCount(id, body);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(message, result.Value);
    }
    /// <summary>
    /// Remove on Register
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldDeletedOneRegister()
    {
        var id = 5;

        this._service.Setup(s => s.RemoveUserRegister(id)).Returns(Task.CompletedTask);

        var res = await this._controller.DeleteRegister(id) as NoContentResult;

        Assert.NotNull(res);
        Assert.Equal(204, res.StatusCode);
    }

    /// <summary>
    /// Update Password
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateThePassword()
    {
        int id = 4;
        var dt = UsersMock.UpdatePasswordDTOMock;
        
        string message = "Password updated successfully";

        this._service.Setup(s => s.UpdatePassword(id, dt)).ReturnsAsync(message);

        var res = await this._controller.UpdatePassword(id, dt);
        var response = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(message, response.Value);
    }

    /// <summary>
    /// Update Roles
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateRoles()
    {
        int id = 5;
        var body = UsersMock.RolesDTOMock;

        string message = "Roles were updated successfully";

        this._service.Setup(s=> s.UpdateRoles(id, body)).ReturnsAsync(message);

        var res = await this._controller.UpdateRoles(id, body);
        var result = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(message, result.Value);
    }
}
