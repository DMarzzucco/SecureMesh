using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using User.Module.Controller;
using User.Module.DTOs;
using User.Module.Model;
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
    /// Test for Register User 
    /// </summary>
    [Fact]
    public async Task ShouldRegisterOneUser()
    {
        var body = UsersMock.CreateUserDTOMOck;
        var user = UsersMock.UserMock;

        this._service.Setup(s => s.RegisterUser(body)).ReturnsAsync(user);
        var res = await this._controller.RegisterUser(body);
        var response = Assert.IsType<CreatedAtActionResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(201, response.StatusCode);
        Assert.Equal(user, response.Value);
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
        var id = 4;
        var user = UsersMock.UserMock;

        this._service.Setup(s => s.UpdateRegister(body, id)).ReturnsAsync(user);
        var res = await this._controller.EditUser(id, body) as NoContentResult;

        Assert.NotNull(res);
        Assert.Equal(204, res.StatusCode);
    }

    /// <summary>
    /// Remove on Register
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldDeletedOneRegister()
    {
        var id = 4;

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
        string oldPassword = "Pr@motheus98";
        string newPassword = "sdAr@motheus34";
        string message = "Password updated successfully";

        this._service.Setup(s => s.UpdatePassword(id, oldPassword, newPassword)).ReturnsAsync(message);

        var res = await this._controller.UpdatePassword(id, oldPassword, newPassword);
        var response = Assert.IsType<OkObjectResult>(res.Result);

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(message, response.Value);
    }
}
