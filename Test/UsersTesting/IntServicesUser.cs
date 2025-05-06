using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.OpenApi.Expressions;
using Moq;
using User.Module.DTOs;
using User.Module.Model;
using User.Module.Repository.Interface;
using User.Module.Service;
using User.Module.Validations.Interface;
using UsersTesting.Mock;

namespace UsersTesting;

public class IntServicesUser
{
    private readonly Mock<IUserRepository> _repository;
    private readonly Mock<IUserValidation> _validation;
    private readonly IMapper _mapper;
    private readonly UserServices _service;

    public IntServicesUser()
    {
        this._repository = new Mock<IUserRepository>();
        this._validation = new Mock<IUserValidation>();

        var conf = new MapperConfiguration(conf =>
        {
            conf.CreateMap<CreateUserDTO, UserModel>();
            conf.CreateMap<UpdateUserDTO, UserModel>();
            conf.CreateMap<UpdateOwnUserDTO, UserModel>()
                .ForMember(d=>d.Password, op => op.Ignore());
            conf.CreateMap<UserModel, UserDTO>();
        });
        this._mapper = conf.CreateMapper();

        this._service = new UserServices(
            this._repository.Object,
            this._mapper,
            this._validation.Object
        );
    }

    /// <summary>
    /// Find User By Id
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldFindUserByValue()
    {
        int id = 4;
        var user = UsersMock.UserMock;

        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        var res = await this._service.FindUserById(id);

        Assert.NotNull(res);
        Assert.Equal(user, res);
    }

    /// <summary>
    /// Get User By Id
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturnOneUserDTO()
    {
        int id = 4;
        var user = UsersMock.UserMock;
        var result = UsersMock.UserMockDTO;

        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        var res = await this._service.GetUserProfileById(id) as UserDTO;

        Assert.NotNull(res);
        Assert.Equal(result.FullName, res.FullName);
    }

    /// <summary>
    /// List of All
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturnOneListOfAllRegister()
    {
        var list = new List<UserDTO> { UsersMock.UserMockDTO };
        var userList = new List<UserModel> { UsersMock.UserMock };

        this._repository.Setup(r => r.ToListAsync()).ReturnsAsync(userList);

        var res = await this._service.ListOfAllRegister();

        Assert.NotNull(res);
    }

    /// <summary>
    /// Register User
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldRegisterOneUser()
    {
        var body = UsersMock.CreateUserDTOMOck;
        var user = UsersMock.UserMock;

        this._validation.Setup(v => v.ValidationDuplicated(body));
        this._validation.Setup(v => v.ValidationEmail(body.Email));
        this._validation.Setup(v => v.ValidateStructurePassword(body.Password));

        this._repository.Setup(r => r.AddChangeAsync(user)).Returns(Task.CompletedTask);

        var res = await this._service.RegisterUser(body) as UserModel;

        Assert.NotNull(res);
        Assert.Equal(user.Username, res.Username);
    }

    /// <summary>
    /// Delete User
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldRemoveOneRegister()
    {
        int id = 5;
        var user = UsersMock.UserMockBasic;

        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        this._repository.Setup(r => r.DeleteAsync(user)).ReturnsAsync(true);

        var res = this._service.RemoveUserRegister(id);

        Assert.NotNull(res);
    }
    /// <summary>
    /// Delete Own Register
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldRemoveOwnRegister()
    {
        int id = 4;
        var dt = UsersMock.PasswordDTOMock;
        var user = UsersMock.UserHashPassMock;
        string message = "User was remove successfully";

        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        this._repository.Setup(r => r.DeleteAsync(user)).ReturnsAsync(true);

        var res = await this._service.RemoveUserRegisterForBasicRoles(id, dt);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }

    /// <summary>
    /// Update Refresh Token
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateToken()
    {
        int id = 4;
        string refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwicm9sIjoiMSIsIm5iZiI6MTc0NTA2OTczMSwiZXhwIjoxNzQ1NTAxNzMxLCJpYXQiOjE3NDUwNjk3MzF9.8Lxp0P4ePIAO9sCHXM8-xO_GUrF-oPQV7__T8Ufz36w";

        var user = UsersMock.UserMock;

        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        this._repository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var res = await this._service.UpdateRefreshToken(id, refreshToken);

        Assert.NotNull(res);
        Assert.Equal(refreshToken, user.RefreshToken);
    }
    /// <summary>
    /// Update the password 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdatePassword()
    {
        int id = 4;
        var dt = UsersMock.UpdatePasswordDTOMock;

        string message = "Password updated successfully";

        var user = UsersMock.UserHashPassMock;

        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        this._validation.Setup(v => v.ValidateStructurePassword(dt.NewPassword));
        this._repository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var res = await this._service.UpdatePassword(id, dt);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }
    /// <summary>
    /// Update Roles
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateRoles()
    {
        int id = 4;
        var roles = UsersMock.RolesDTOMock;
        var user = UsersMock.UserMock;
        string message = "Roles were updated successfully";

        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        this._repository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var res = await this._service.UpdateRoles(id, roles);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }
    /// <summary>
    /// Update Register
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateOneRegister()
    {
        // Given
        var body = UsersMock.UpdateUserDTOMOck;
        int id = 5;
        var user = UsersMock.UserMockBasic;
        // When
        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        this._validation.Setup(v => v.ValidationEmail(body.Email));
        this._repository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var res = await this._service.UpdateRegister(body, id);
        // Then

        Assert.NotNull(res);
        Assert.Equal(body.Username, res.Username);
    }
    /// <summary>
    /// Update own register
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldUpdateOwnRegister()
    {
        int id = 4;
        var body = UsersMock.UpdateOwnUserDTOMock;

        var user = UsersMock.UserHashPassMock;
        string message = "Your reforms was saved successfully";

        this._repository.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(user);
        this._validation.Setup(v => v.ValidationEmail(body.Email));
        this._repository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var res = await this._service.UpdateOwnRegister(id, body);

        Assert.NotNull(res);
        Assert.Equal(message, res);
    }
}
