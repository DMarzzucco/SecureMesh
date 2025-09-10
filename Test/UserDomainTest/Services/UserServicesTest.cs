using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using User.Module.Model;
using User.Module.Repository.Interface;
using User.Module.Service;
using User.Module.Validations.Interface;
using User.Utils.Exceptions;
using UserDomainTest.Mock;

namespace UserDomainTest.Services;

public class UserServicesTest
{
    private readonly Mock<IUserRepository> _repository;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IUserValidation> _userValidation;
    private readonly UserServices _service;

    public UserServicesTest()
    {
        this._repository = new Mock<IUserRepository>();
        this._mapper = new Mock<IMapper>();
        this._userValidation = new Mock<IUserValidation>();

        this._service = new UserServices(
            this._repository.Object,
            this._mapper.Object,
            this._userValidation.Object
        );
    }

    [Fact]
    public async Task RegisterUser_Should_Save_User()
    {
        var dto = UserDomainMocks.CreateUserDTOMOck;
        var user = UserDomainMocks.UserMock;

        this._mapper.Setup(m => m.Map<UserModel>(dto)).Returns(user);

        this._repository.Setup(r => r.AddChangeAsync(It.IsAny<UserModel>()))
            .Returns(Task.CompletedTask);

        var result = await this._service.RegisterUser(dto);

        Assert.Equal(dto.Username, result.Username);
        Assert.NotNull(result.Password);

        this._repository.Verify(r => r.AddChangeAsync(It.IsAny<UserModel>()), Times.Once);
    }

    [Fact]
    public async Task RemoveUserRegister_Should_Delete_User_When_Not_Admin()
    {
        var user = UserDomainMocks.UserNotAdminMock;

        this._repository.Setup(r => r.FindByIdAsync(2)).ReturnsAsync(user);

        var result = await this._service.RemoveUserRegister(2);

        Assert.Equal($"User {user.FullName} was deleted successfully", result);
        this._repository.Verify(r => r.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task RemoveUserRegister_Should_Throw_Forbidden_When_Admin()
    {
        var user = UserDomainMocks.UserMock;

        this._repository.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(user);

        await Assert.ThrowsAsync<ForbiddenExceptions>(() => _service.RemoveUserRegister(1));
    }

    [Fact]
    public async Task UpdatePassword_Should_Throw_When_OldPassword_Wrong()
    {
        var user = UserDomainMocks.UserMock;
        var hasher = new PasswordHasher<UserModel>();

        user.Password = hasher.HashPassword(user, "old-pass");

        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        await Assert.ThrowsAsync<ForbiddenExceptions>(() =>
            this._service.UpdatePassword(4, "wrong-old", "new-pass123"));
    }

    [Fact]
    public async Task UpdateEmail_Should_Save_New_Email()
    {
        var user = UserDomainMocks.UserMock;
        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        var result = await this._service.UpdateEmail(4, "new@email.com");

        Assert.Equal("new@email.com", result.Email);
        this._repository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task VerifyEmail_Should_Verify_New_Email()
    {
        string password = UserDomainMocks.UserMock.Password;
        string newEmail = "newEmail@hotmail.com";

        var user = UserDomainMocks.UserHashPassMock;
        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        var hasher = new PasswordHasher<UserModel>();

        hasher.VerifyHashedPassword(user, user.Password, password);

        this._userValidation.Setup(v => v.ValidationEmail(newEmail));
        this._userValidation.Setup(v => v.ValidateEmailDuplicate(newEmail)).Returns(Task.CompletedTask);

        var result = await this._service.VerifyNewEmailParameters(4, password, newEmail);

        Assert.Equal(user.Username, result.Username);
        this._repository.Verify(r => r.FindByIdAsync(4), Times.Once);
    }

    [Fact]
    public async Task VerifyEmail_Should_Throw_When_User_NotFoundAsync()
    {
        string password = UserDomainMocks.UserMock.Password;

        var user = UserDomainMocks.UserHashPassMock;
        var hasher = new PasswordHasher<UserModel>();

        hasher.VerifyHashedPassword(user, user.Password, password);

        await Assert.ThrowsAsync<NotFoundExceptions>(() =>
            this._service.VerifyNewEmailParameters(3, password, "newEmail_@gmail.com"));
    }

    [Fact]
    public async Task VerifyEmail_Should_Throw_When_Password_Wrong()
    {
        string password = UserDomainMocks.UserHashPassMock.Password;

        var user = UserDomainMocks.UserHashPassMock;
        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        await Assert.ThrowsAsync<ForbiddenExceptions>(() =>
            this._service.VerifyNewEmailParameters(4, password, "newEmail_@gmail.com"));
    }

    [Fact]
    public async Task UpdateOwnRegister_Should_Return_A_Message()
    {
        var dto = UserDomainMocks.UpdateOwnUserDTOMock;

        var user = UserDomainMocks.UserMock;
        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        string message = "Your reforms was saved successfully";

        this._userValidation.Setup(v => v.ValidationUsernameDuplicated(dto.Username))
            .Returns(Task.CompletedTask);

        this._repository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var result = await this._service.UpdateOwnRegister(4, dto);
        Assert.Equal(message, result);

        this._repository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateRoles_Should_Return_A_Message()
    {
        var user = UserDomainMocks.UserNotAdminMock;
        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        string message = "Roles were updated successfully";

        this._repository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var result = await this._service.UpdateRoles(4, User.ROLES.Admin);
        Assert.Equal(message, result);

        this._repository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateRoles_Should_Throw_When_Roles_Is_The_Same()
    {
        var user = UserDomainMocks.UserNotAdminMock;
        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        await Assert.ThrowsAsync<BadRequestExceptions>(() =>
            this._service.UpdateRoles(4, User.ROLES.Creator));
    }

    [Fact]
    public async Task FindValueByKey_Should_Return_UserModel()
    {
        string username = UserDomainMocks.UserMock.Username;

        var user = UserDomainMocks.UserMock;
        this._repository.Setup(r => r.FindByKey("Username", username)).ReturnsAsync(user);

        var result = await this._service.FindValueByKey("Username", username);
        Assert.Equal(user.Username, username);

        this._repository.Verify(r => r.FindByKey("Username", username), Times.Once);
    }

    [Fact]
    public async Task FindValueByKey_Should_Throw_User_Not_Found()
    {
        string username = "usernotexist";
        var user = UserDomainMocks.UserMock;

        await Assert.ThrowsAsync<NotFoundExceptions>(() =>
            this._service.FindValueByKey("Username", username));
    }

    [Fact]
    public async Task ReturnPasswordAsync_Should_Return_UserModel()
    {
        string newPassword = "Sr@motheus98";

        var user = UserDomainMocks.UserHashPassMock;
        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        var hasher = new PasswordHasher<UserModel>();

        this._userValidation.Setup(v => v.ValidateStructurePassword(newPassword));

        hasher.VerifyHashedPassword(user, user.Password, newPassword);
        hasher.HashPassword(user, newPassword);

        this._repository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var result = await this._service.ReturnPasswordAsync(4, newPassword);
        Assert.Equal(user.Username, result.Username);

        this._repository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task ReturnPasswordAsync_Should_Throws_Password_Is_Same_at_the_old()
    {
        var user = UserDomainMocks.UserHashPassMock;
        this._repository.Setup(r => r.FindByIdAsync(4)).ReturnsAsync(user);

        var hasher = new PasswordHasher<UserModel>();

        string newPassword = UserDomainMocks.UserMock.Username;
        user.Password = hasher.HashPassword(user, newPassword);

        this._userValidation.Setup(v => v.ValidateStructurePassword(newPassword));

        await Assert.ThrowsAsync<ConflictExceptions>(() =>
            this._service.ReturnPasswordAsync(4, newPassword));
    }
}
