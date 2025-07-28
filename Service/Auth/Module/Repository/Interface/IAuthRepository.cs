using Auth.Module.Model;
using System;

namespace Auth.Module.Repository.Interface;

public interface IAuthRepository
{
    Task<AuthModel?> FindAuthByUserId(int userId);
    Task SaveAuth(int userId);
    Task<bool> UpdateAsync(AuthModel body);
    Task RemoveAuthRealtionAsync(AuthModel body);
}
