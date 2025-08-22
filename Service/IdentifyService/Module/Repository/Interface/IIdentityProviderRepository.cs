using IdentifyService.Module.Model;
using System;

namespace IdentifyService.Module.Repository.Interface;

public interface IIdentityProviderRepository
{
    Task<AuthModel?> FindAuthByUserId(int userId);
    Task SaveAuth(int userId);
    Task<bool> UpdateAsync(AuthModel body);
    Task RemoveAuthRealtionAsync(AuthModel body);
}
