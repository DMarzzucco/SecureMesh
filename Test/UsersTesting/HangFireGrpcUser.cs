using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Moq;
using User;
using User.Module.Repository.Interface;
using User.Module.Stubs;
using UsersTesting.Mock;

namespace UsersTesting;

public class HangFireGrpcUser
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly UserHangFireServicesImpl _services;

    public HangFireGrpcUser()
    {
        this._userRepository = new Mock<IUserRepository>();
        this._services = new UserHangFireServicesImpl(this._userRepository.Object);
    }
    /// <summary>
    /// Counted of Deleted
    /// </summary>
    [Fact]
    public async Task ShouldInitACountedToDeletedAccount()
    {
        var user = UsersMock.UserMock;
        int id = 4;

        var request = new UserHangFire.Protos.UserRequest { Id = id, };

        var fakeContext = TestServerCallContext.Create(
            method: "CountedDeleted",
            host: null,
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "localhost",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: (metadata) => Task.CompletedTask,
            writeOptionsGetter: () => null,
            writeOptionsSetter: (options) => { }
        );

        this._userRepository.Setup(r => r.FindByIdAsync(request.Id)).ReturnsAsync(user);
        this._userRepository.Setup(r => r.DeleteAsync(user)).ReturnsAsync(true);

        var res = await this._services.CountedDeleted(request, fakeContext);

        Assert.IsType<Empty>(res);
    }
}
