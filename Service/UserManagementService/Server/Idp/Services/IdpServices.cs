using System;
using IdentifyService.Proto.Server;
using UserManagementService.Server.Idp.Services.Interfaces;

namespace UserManagementService.Server.Idp.Services;

public class IdpServices(RemoveIdpRelationService.RemoveIdpRelationServiceClient client) : IIdpServices
{
    private readonly RemoveIdpRelationService.RemoveIdpRelationServiceClient client = client;

    public async Task RemoveIdpRelation(int userId)
    {
        var request = new UserIdRequest { UserId = userId };
        await this.client.InvokeDeletionRelationIdpAsync(request);
    }
}
