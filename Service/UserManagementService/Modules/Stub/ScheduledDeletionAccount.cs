using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UserManagementService.Modules.Repository.Interfaces;
using UserManagementService.Proto.Server;
using UserManagementService.Server.Idp.Services.Interfaces;
using UserManagementService.Server.Sessions.Services.Interfaces;
using UserManagementService.Server.Users.Service.Interfaces;

namespace UserManagementService.Modules.Stub;

public class ScheduledDeletionAccount : ScheduledDeletionCountService.ScheduledDeletionCountServiceBase
{
    private readonly IManagementUserRepository _managementUserRepository;
    private readonly ISessionManagementServices _sessionManagement;
    private readonly IIdpServices _idpServices;
    private readonly IUserService _userService;

    public ScheduledDeletionAccount(IManagementUserRepository managementUserRepository, ISessionManagementServices sessionManagement, IUserService userService, IIdpServices idpServices)
    {
        _managementUserRepository = managementUserRepository;
        _sessionManagement = sessionManagement;
        _userService = userService;
        _idpServices = idpServices;
    }

    public override async Task<Empty> InvokeCounted(UserIdToDeleteRequest request, ServerCallContext context)
    {
        var ms = await this._managementUserRepository.GetRelationManagementByUserId(request.UserId) ??
            throw new RpcException(new Status(StatusCode.NotFound, "Not Found user"));

        if (!ms.IsDeleted || ms.DeletedAt == null)
            return new Empty();

        if (DateTime.UtcNow < ms.DeletedAt.Value.AddMinutes(10))
            return new Empty();

        await this._sessionManagement.RemoveAllSessionsByUserId(request.UserId);
        await this._idpServices.RemoveIdpRelation(request.UserId);
        await this._managementUserRepository.DeleteRelationManagementByUserId(ms);
        await this._userService.RemoveUser(request.UserId);

        return new Empty();
    }
}
