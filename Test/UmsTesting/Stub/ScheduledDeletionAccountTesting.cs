using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using UmsTesting.Helper;
using UserManagementService.Modules.Models;
using UserManagementService.Modules.Repository.Interfaces;
using UserManagementService.Modules.Stub;
using UserManagementService.Proto.Server;
using UserManagementService.Server.Idp.Services.Interfaces;
using UserManagementService.Server.Sessions.Services.Interfaces;
using UserManagementService.Server.Users.Service.Interfaces;

namespace UmsTesting.Stub;

public class ScheduledDeletionAccountTesting
{
    private readonly new Mock<IManagementUserRepository> _repository;
    private readonly new Mock<ISessionManagementServices> _sessionManagement;
    private readonly new Mock<IIdpServices> _idpServices;
    private readonly new Mock<IUserService> _userService;
    private readonly ScheduledDeletionAccount _service;
    private readonly ServerCallContext _context;

    public ScheduledDeletionAccountTesting()
    {
        this._repository = new Mock<IManagementUserRepository>();
        this._sessionManagement = new Mock<ISessionManagementServices>();
        this._idpServices = new Mock<IIdpServices>();
        this._userService = new Mock<IUserService>();

        this._service = new ScheduledDeletionAccount(
            this._repository.Object,
            this._sessionManagement.Object,
            this._userService.Object,
            this._idpServices.Object
        );

        this._context = TestServerCallContext.Create();
    }

    [Fact]
    public async Task InvokeCounted_Should_Throw_When_User_NotFound()
    {
        var ms = new ManagementUserModel { Id = 1, UserId = 4 };

        var request = new UserIdToDeleteRequest { UserId = 4 };

        this._repository.Setup(r => r.GetRelationManagementByUserId(3))
                 .ReturnsAsync(ms);

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            this._service.InvokeCounted(request, _context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task InvokeCounted_Should_Return_Empty_When_User_Not_Deleted()
    {
        var request = new UserIdToDeleteRequest { UserId = 4 };
        var relation = new ManagementUserModel { IsDeleted = false, DeletedAt = null };

        this._repository.Setup(r => r.GetRelationManagementByUserId(4))
                 .ReturnsAsync(relation);

        var result = await _service.InvokeCounted(request, _context);

        Assert.IsType<Empty>(result);

        this._sessionManagement.Verify(s => s.RemoveAllSessionsByUserId(It.IsAny<int>()), Times.Never);
        this._idpServices.Verify(i => i.RemoveIdpRelation(It.IsAny<int>()), Times.Never);
        this._userService.Verify(u => u.RemoveUser(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task InvokeCounted_Should_Return_Empty_When_Deleted_But_Within_Grace_Period()
    {
        var request = new UserIdToDeleteRequest { UserId = 4 };
        var relation = new ManagementUserModel { IsDeleted = true, DeletedAt = DateTime.UtcNow };

        this._repository.Setup(r => r.GetRelationManagementByUserId(4))
                 .ReturnsAsync(relation);

        var result = await this._service.InvokeCounted(request, _context);

        Assert.IsType<Empty>(result);

        this._sessionManagement.Verify(s => s.RemoveAllSessionsByUserId(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task InvokeCounted_Should_Delete_When_Deleted_And_Expired()
    {
        var request = new UserIdToDeleteRequest { UserId = 4 };
        var relation = new ManagementUserModel { IsDeleted = true, DeletedAt = DateTime.UtcNow.AddMinutes(-15) };

        this._repository.Setup(r => r.GetRelationManagementByUserId(4))
                 .ReturnsAsync(relation);

        var result = await this._service.InvokeCounted(request, _context);

        Assert.IsType<Empty>(result);

        this._sessionManagement.Verify(s => s.RemoveAllSessionsByUserId(4), Times.Once);
        this._idpServices.Verify(i => i.RemoveIdpRelation(4), Times.Once);
        this._repository.Verify(r => r.DeleteRelationManagementByUserId(relation), Times.Once);
        this._userService.Verify(u => u.RemoveUser(4), Times.Once);
    }
}
