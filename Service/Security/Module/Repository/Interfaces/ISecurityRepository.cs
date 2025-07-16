using Security.Module.Model;
using System;

namespace Security.Module.Repository.Interfaces;

public interface ISecurityRepository
{
    Task<SecurityModel> FindByUserId(int id);
    Task<SecurityModel> FindRelationById(int id);
    Task<IEnumerable<SecurityModel>> ListOfAllAsync();
    Task<bool> RemoveSessionAsync(SecurityModel body);
    Task<bool> UpdateState(SecurityModel body);
    Task SaveSession(SecurityModel body);
}
