using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Moq;
using User;
using User.Module.Repository.Interface;
using User.Module.Service.Interface;
using User.Module.Stubs;
using User.Module.Stubs.Handlers;
using User.Module.Stubs.Maps;
using UsersTesting.Mock;

namespace UsersTesting;

public class GrpcUserService
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<MapResponseGrpc> _mapper;
    private readonly Mock<HandlerGrpcExceptions> _handlerGrpcExtensions;
    private readonly UserServiceGrpcImpl _service;

    public GrpcUserService()
    {
        this._userService = new Mock<IUserService>();
        this._userRepository = new Mock<IUserRepository>();
        this._mapper = new Mock<MapResponseGrpc>();
        this._handlerGrpcExtensions = new Mock<HandlerGrpcExceptions>();

        this._service = new UserServiceGrpcImpl(
            this._userService.Object,
            this._userRepository.Object,
            this._mapper.Object,
            this._handlerGrpcExtensions.Object
        );
    }

    /// <summary>
    /// Cancel Deleted Operation
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void ShouldCancelDeletedOperation()
    {
        //var user = UsersMock.UserMock;
        //int id = 4;

        //var request = new UserRequest { Id = id, };

        //var fakeContext = TestServerCallContext.Create(
        //    method: "CancelationOperationAuth",
        //    host: null,
        //    deadline: DateTime.UtcNow.AddMinutes(1),
        //    requestHeaders: new Metadata(),
        //    cancellationToken: CancellationToken.None,
        //    peer: "localhost",
        //    authContext: null,
        //    contextPropagationToken: null,
        //    writeHeadersFunc: (metadata) => Task.CompletedTask,
        //    writeOptionsGetter: () => null,
        //    writeOptionsSetter: (options) => { }
        //);

        //this._userRepository.Setup(r => r.FindByIdAsync(request.Id)).ReturnsAsync(user);
        //this._hangFireServices.Setup(h => h.DeletedScheduledJob(user.ScheduledDeletionJobId));
        //this._userRepository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        //var res = await this._service.CancelationOperationAuth(request, fakeContext);

        //Assert.IsType<Empty>(res);
    }
}
