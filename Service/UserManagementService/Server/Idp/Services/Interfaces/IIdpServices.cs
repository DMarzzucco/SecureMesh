using System;

namespace UserManagementService.Server.Idp.Services.Interfaces;

public interface IIdpServices
{
    public Task RemoveIdpRelation(int userId);
}
