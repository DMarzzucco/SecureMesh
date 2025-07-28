using Security.Module.Model;
using System;

namespace Security.Module.Repository.Interfaces;

public interface ISecurityRepository
{
    Task<SessionModel?> FindByUserId(int id);
    Task<SessionModel?> FindSession(int userId, string ip, string userAgent, string location);
    Task<SessionModel?> FindSessionById(int id);
    Task<IEnumerable<SessionModel>> FindAllSessionsByUserId(int userId);
    Task<bool> RemoveSessionAsync(SessionModel body);
    Task RemoveAllSessionsExceptCurrent(int userId, int currentSessoinId);
    Task<bool> UpdateState(SessionModel body);
    Task SaveSession(SessionModel body);
    Task RemoveAllSessionsByUserId(int userId);
}
