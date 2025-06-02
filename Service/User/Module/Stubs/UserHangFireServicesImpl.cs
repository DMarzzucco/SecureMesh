using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using User.Module.Repository.Interface;
using UserHangFire.Protos;

namespace User.Module.Stubs;

public class UserHangFireServicesImpl(IUserRepository userRepository) : UserHangFireService.UserHangFireServiceBase
{
    private readonly IUserRepository userRepository = userRepository;

    /// <summary>
    /// Counted Deleted
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<Empty> CountedDeleted(UserHangFire.Protos.UserRequest request, ServerCallContext context)
    {
        var user = await this.userRepository.FindByIdAsync(request.Id) ??
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        if (!user.IsDeleted || user.DeletedAt == null)
            return new Empty();
            
        if (DateTime.UtcNow < user.DeletedAt.Value.AddMinutes(10))
            return new Empty();

        await this.userRepository.DeleteAsync(user);

        return new Empty();
    }
}
