using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Moq;
using SessionsManagement.Module.Model;
using SessionsManagement.Module.Repository.Interfaces;
using SessionsManagement.Protos;
using SessionsManagement.Services;
using SessionsManagementTesting.Helper;
using SessionsManagementTesting.Mock;

namespace SessionsManagementTesting;

public class SessionManagementServiceTest
{
    private readonly new Mock<IManagementSessionsRepository> _repository;
    private readonly ManagementSessionsServices _service;
    private readonly ServerCallContext _context;

    public SessionManagementServiceTest()
    {
        this._repository = new Mock<IManagementSessionsRepository>();
        this._service = new ManagementSessionsServices(this._repository.Object);
        this._context = TestServerCallContext.Create();
    }

    [Fact]
    public async Task DeleteAllSessionsByUserId_Should_Call_Repository()
    {
        var request = new UserIdRequest { UserId = 4 };

        var result = await _service.DeleteAllSessionsByUserId(request, _context);

        this._repository.Verify(r => r.RemoveAllSessionsByUserId(4), Times.Once);

        Assert.IsType<Empty>(result);
    }

    
    [Fact]
    public async Task DeleteSessionById_Should_Throw_When_Not_Found()
    {
        int sessionId = 2;
        var request = new IdRequest { Id = 2 };

        this._repository.Setup(r => r.FindSessionById(sessionId))
                       .ReturnsAsync((SessionModel?)null);

        var ex = await Assert.ThrowsAsync<RpcException>(() => _service.DeleteSessionById(request, _context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteSessionById_Should_Remove_When_Found()
    {
        var session = SSMMocks.SessionModelMock;

        this._repository.Setup(r => r.FindSessionById(2)).ReturnsAsync(session);

        var request = new IdRequest { Id = 2 };
        var result = await _service.DeleteSessionById(request, _context);

        this._repository.Verify(r => r.RemoveSessionAsync(session), Times.Once);
        Assert.IsType<Empty>(result);
    }

    [Fact]
    public async Task FindSessionByProps_Should_Return_NotFound_When_Null()
    {
        var session = SSMMocks.SessionModelMock;

        var request = new SessionsPropsRequest { UserId = 4, Ip = "1.4.5.7" };

        this._repository.Setup(r => r.FindSession(session.UserId, session.Ip, session.UserAgent, session.Location))
                       .ReturnsAsync((SessionModel?)null);

        var result = await _service.FindSessionByProps(request, _context);

        Assert.NotNull(result.Reason);
        Assert.Equal("Not found session in this user", result.Reason.Reason);
    }

    [Fact]
    public async Task FindSessionByProps_Should_Return_Session_When_Found()
    {
        var session = SSMMocks.SessionModelMock;
        var request = SSMMocks.SessionsPropsRequestMock;

        this._repository.Setup(r => r.FindSession(session.UserId, session.Ip, session.UserAgent, session.Location))
                       .ReturnsAsync(session);

        var result = await _service.FindSessionByProps(request, _context);

        Assert.NotNull(result.Session);
        Assert.Equal(session.Id, result.Session.Id);
    }

    [Fact]
    public async Task SaveSession_Should_Call_Repository_And_Return_Id()
    {
        var request = SSMMocks.SessionsPropsRequestMock;

        this._repository.Setup(r => r.SaveSession(It.IsAny<SessionModel>()))
                       .Callback<SessionModel>(s => s.Id = 2)
                       .Returns(Task.CompletedTask);

        var result = await _service.SaveSession(request, _context);

        this._repository.Verify(r => r.SaveSession(It.IsAny<SessionModel>()), Times.Once);
        Assert.Equal(2, result.Id);
    }
}
