using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using User;
using User.Module.DTOs;
using User.Module.Model;
using User.Module.Repository.Interface;
using User.Module.Service.Interface;
using User.Module.Stubs;
using User.Module.Stubs.Handlers;
using User.Module.Stubs.Maps;
using UserDomainTest.Helper;
using UserDomainTest.Mock;

namespace UserDomainTest.Stub;

public class UserServiceGrpcImplTest
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IUserRepository> _repository;
    private readonly Mock<MapResponseGrpc> _mapper;
    private readonly Mock<HandlerGrpcExceptions> _handlerGrpcExceptions;
    private readonly UserServiceGrpcImpl _service;
    private readonly ServerCallContext _context;

    public UserServiceGrpcImplTest()
    {
        this._userService = new Mock<IUserService>();
        this._repository = new Mock<IUserRepository>();
        this._mapper = new Mock<MapResponseGrpc>();
        this._handlerGrpcExceptions = new Mock<HandlerGrpcExceptions>();

        this._service = new UserServiceGrpcImpl(
            this._userService.Object,
            this._repository.Object,
            this._mapper.Object,
            this._handlerGrpcExceptions.Object
        );

        this._context = TestServerCallContext.Create();
    }

    [Fact]
    public async Task DeleteAnyAccount_Should_Return_MessageResponse()
    {
        var request = new UserRequest { Id = 4 };
        var user = UserDomainMocks.UserMock;

        string message = $"User {user.FullName} was deleted successfully";

        this._userService.Setup(us => us.RemoveUserRegister(4)).ReturnsAsync(message);

        var response = new MessageResponse { Message = message };

        var result = await this._service.DeleteAnyAccount(request, this._context);
        Assert.Equal(response, result);

        this._userService.Verify(us => us.RemoveUserRegister(4), Times.Once);
    }

    [Fact]
    public async Task GetListOfAllUsers_Should_Return_ListOfUserResponse()
    {
        var list = new List<UserModel> { UserDomainMocks.UserHashPassMock };
        this._repository.Setup(r => r.ToListAsync()).ReturnsAsync(list);

        var request = new Empty();
        var response = new List<AuthUserResponse> { UserDomainMocks.AuthUserResponseMock };

        var result = await this._service.GetListOfAllUsers(request, this._context);
        Assert.NotNull(result);
        Assert.Equal("derkmarzz77", result.User[0].Username);

        this._repository.Verify(r => r.ToListAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateRolesUser_Should_Return_MessageResponse()
    {
        var user = UserDomainMocks.UserMock;
        var request = new UpdateRolesRequest { Id = 4, NewRoles = ROLES.Basic };

        string message = "Roles were updated successfully";

        this._userService.Setup(us => us.UpdateRoles(request.Id, request.NewRoles))
            .ReturnsAsync(message);

        var response = new MessageResponse { Message = message };

        var result = await this._service.UpdateRolesUser(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(response, result);

        this._userService.Verify(us => us.UpdateRoles(4, request.NewRoles), Times.Once);
    }

    [Fact]
    public async Task UpdateOwnAccount_Should_Return_MessageResponse()
    {
        var user = UserDomainMocks.UserMock;
        var request = new UpdateOwnUserDTORequest
        {
            Id = 4,
            Password = user.Password,
            FullName = user.FullName,
            Username = user.Username
        };

        string message = "Your reforms was saved successfully";

        this._userService.Setup(us => us.UpdateOwnRegister(4, It.Is<UpdateOwnUserDTO>(dto =>
            dto.Password == request.Password &&
            dto.FullName == request.FullName &&
            dto.Username == request.Username
        ))).ReturnsAsync(message);

        var response = new MessageResponse { Message = message };

        var result = await this._service.UpdateOwnAccount(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(response.Message, result.Message);

        this._userService.Verify(us =>
            us.UpdateOwnRegister(4, It.IsAny<UpdateOwnUserDTO>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_Should_Return_Empty_Value()
    {
        var user = UserDomainMocks.UserMock;
        var request = new UserRequest { Id = user.Id };

        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);
        this._repository.Setup(r => r.DeleteAsync(user)).ReturnsAsync(true);

        var result = await this._service.DeleteAccount(request, this._context);
        Assert.Equal(new Empty(), result);

        this._repository.Verify(r => r.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_Should_Throw_Not_Found_User()
    {
        var request = new UserRequest { Id = 4 };

        await Assert.ThrowsAsync<RpcException>(() =>
            this._service.DeleteAccount(request, this._context));
    }

    [Fact]
    public async Task UpdatePasswordAuth_Should_Return_MessageResponse()
    {
        var user = UserDomainMocks.UserMock;
        var request = new UpdatePasswordDTORequest { Id = 4, Password = user.Password, NewPassword = "Sr@motheus98" };

        string message = "Password updated successfully";

        this._userService.Setup(us => us.UpdatePassword(request.Id, request.Password, request.NewPassword)).ReturnsAsync(message);

        var result = await this._service.UpdatePasswordAuth(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(message, result.Message);

        this._userService.Verify(us =>
            us.UpdatePassword(4, request.Password, request.NewPassword), Times.Once);
    }

    [Fact]
    public async Task RegisterUserInAuth_Should_Return_UserModel()
    {
        var user = UserDomainMocks.UserMock;
        var request = new CreateUserRequest
        {
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            Roles = user.Roles
        };

        this._userService.Setup(us => us.RegisterUser(It.Is<CreateUserDTO>(dto =>
            dto.FullName == request.FullName &&
            dto.Username == request.Username &&
            dto.Email == request.Email &&
            dto.Password == request.Password &&
            dto.Roles == request.Roles)))
            .ReturnsAsync(user);

        var result = await this._service.RegisterUserInAuth(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.User.Username);

        this._userService.Verify(us =>
            us.RegisterUser(It.IsAny<CreateUserDTO>()), Times.Once);
    }

    [Fact]
    public async Task VerifyNewEmailParameters_Should_Return_UserModel()
    {
        var user = UserDomainMocks.UserMock;
        var request = new NewEmailDTORequest
        {
            Id = 4,
            Password = user.Password,
            NewEmail = "new_email@hotmail.com"
        };

        this._userService.Setup(us =>
            us.VerifyNewEmailParameters(request.Id, request.Password, request.NewEmail))
                .ReturnsAsync(user);

        var result = await this._service.VerifyNewEmailParameters(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.User.Username);

        this._userService.Verify(us =>
            us.VerifyNewEmailParameters(4, request.Password, request.NewEmail), Times.Once);
    }

    [Fact]
    public async Task UpdateEmailAddress_Should_Return_A_UserModel()
    {
        var user = UserDomainMocks.UserMock;
        var request = new UpdateEmailRequest { Id = 4, NewEmail = "newEmail@gmail.com" };

        this._userService.Setup(us =>
            us.UpdateEmail(request.Id, request.NewEmail)).ReturnsAsync(user);

        var result = await this._service.UpdateEmailAddress(request, this._context);

        Assert.NotNull(result);
        Assert.Equal(user.Username, result.User.Username);

        this._userService.Verify(us => us.UpdateEmail(4, request.NewEmail), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdForAuth_Should_Return_A_UserModel()
    {
        var user = UserDomainMocks.UserMock;
        var request = new UserRequest { Id = 4 };
        this._repository.Setup(r =>
            r.FindByIdAsync(4))
                .ReturnsAsync(user);

        var result = await this._service.GetUserByIdForAuth(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);

        this._repository.Verify(r => r.FindByIdAsync(4), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailForAuth_Should_Return_A_UserModel()
    {
        var user = UserDomainMocks.UserMock;
        var request = new UserEmailRequest { Email = user.Email };

        this._repository.Setup(r => r.FindByEmailAsync(request.Email)).ReturnsAsync(user);

        var result = await this._service.GetUserByEmailForAuth(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.User.Username);

        this._repository.Verify(r => r.FindByEmailAsync(user.Email), Times.Once);
    }

    [Fact]
    public async Task ReturnPasswordForAuth_Should_Return_A_UserModel()
    {
        var user = UserDomainMocks.UserMock;
        var request = new PasswordDTORequest { Id = 4, Password = user.Password };

        this._userService.Setup(us => us.ReturnPasswordAsync(4, request.Password))
            .ReturnsAsync(user);

        var result = await this._service.ReturnPasswordForAuth(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.User.Username);

        this._userService.Verify(us => us.ReturnPasswordAsync(4, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task FindByValueForAuth_Should_Return_A_UserModel()
    {
        var user = UserDomainMocks.UserMock;
        var request = new ValueKeysRequest { Key = "Username", StringValue = user.Username };

        this._userService.Setup(us => us.FindValueByKey(request.Key, request.StringValue)).ReturnsAsync(user);

        var result = await this._service.FindByValueForAuth(request, this._context);
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.User.Username);

        this._userService.Verify(us =>
            us.FindValueByKey(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

}
