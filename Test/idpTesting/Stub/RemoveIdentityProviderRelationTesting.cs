using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentifyService.Module.Model;
using IdentifyService.Module.Repository.Interface;
using IdentifyService.Module.Stub;
using IdentifyService.Proto.Server;
using idpTesting.Helper;
using Moq;

namespace idpTesting.Stub;

public class RemoveIdentityProviderRelationTesting
{
    private readonly new Mock<IIdentityProviderRepository> _repository;
    private readonly RemoveIdentityProviderRelation _service;
    private readonly ServerCallContext _context;

    public RemoveIdentityProviderRelationTesting()
    {
        this._repository = new Mock<IIdentityProviderRepository>();
        this._service = new RemoveIdentityProviderRelation(this._repository.Object);

        this._context = TestServerCallContext.Create();
    }
    
    [Fact]
    public async Task InvokeDeletionRelationIdp_Should_Throw_When_User_NotFound()
    {
        var request = new UserIdRequest { UserId = 4 };

        this._repository.Setup(r => r.FindAuthByUserId(3))
                 .ReturnsAsync((AuthModel?)null);

        var ex = await Assert.ThrowsAsync<RpcException>(
            () => this._service.InvokeDeletionRelationIdp(request, _context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);

        this._repository.Verify(r => r.RemoveAuthRealtionAsync(It.IsAny<AuthModel>()), Times.Never);
    }

    [Fact]
    public async Task InvokeDeletionRelationIdp_Should_Remove_When_User_Exists()
    {
        var relation = new AuthModel { UserId = 4 };
        var request = new UserIdRequest { UserId = 4 };

        this._repository.Setup(r => r.FindAuthByUserId(4))
                 .ReturnsAsync(relation);

        var result = await this._service.InvokeDeletionRelationIdp(request, _context);

        this._repository.Verify(r => r.RemoveAuthRealtionAsync(relation), Times.Once);
        
        Assert.IsType<Empty>(result);
    }
}
