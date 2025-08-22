using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentifyService.Module.Repository.Interface;
using IdentifyService.Proto.Server;

namespace IdentifyService.Module.Stub;

public class RemoveIdentityProviderRelation(IIdentityProviderRepository repository) : RemoveIdpRelationService.RemoveIdpRelationServiceBase
{
    private readonly IIdentityProviderRepository repository = repository;

    /// <summary>
    /// Remove Relation IDP
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<Empty> InvokeDeletionRelationIdp(UserIdRequest request, ServerCallContext context)
    {
        var idp = await this.repository.FindAuthByUserId(request.UserId) ??
            throw new RpcException(new Status(StatusCode.NotFound, "Not found user"));

        await this.repository.RemoveAuthRealtionAsync(idp);

        return new Empty();
    }
}
