using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Auth.Server.Users.Service.Interfaces;
using Auth.Module.Repository.Interface;
using AuthHangFire.Proto;
using Auth.Server.Security.Service.Interfaces;

namespace Auth.Module.Stub.Hangfire;

public class AuthHangfireServiceGrpc(IUserService userService, IAuthRepository repository, ISecurityService securityService) : AuthHangFireService.AuthHangFireServiceBase
{
    private readonly IUserService userService = userService;
    private readonly IAuthRepository repository = repository;
    private readonly ISecurityService sessionService = securityService;

    public override async Task<Empty> CountedDeleted(AuthRequest request, ServerCallContext context)
    {
        var auth = await this.repository.FindAuthByUserId(request.Id) ??
                        throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        if (!auth.IsDeleted || auth.DeletedAt == null)
            return new Empty();

        if (DateTime.UtcNow < auth.DeletedAt.Value.AddMinutes(10))
            return new Empty();

        await this.sessionService.RemoveAllSessionsByUserId(request.Id);
        await this.repository.RemoveAuthRealtionAsync(auth);
        await this.userService.RemoveUser(request.Id);
        
        return new Empty();
    }
}
