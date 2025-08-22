using SessionsManagement.Module.Model;
using System;

namespace SessionsManagement.Module.Repository.Interfaces;

public interface IManagementSessionsRepository
{
    Task<SessionModel?> FindSession(int userId, string ip, string userAgent, string location);
    Task<SessionModel?> FindSessionById(int id);
    Task<IEnumerable<SessionModel>> FindAllSessionsByUserId(int userId);
    Task<bool> RemoveSessionAsync(SessionModel body);
    Task RemoveAllSessionsExceptCurrent(int userId, int currentSessoinId);
    Task SaveSession(SessionModel body);
    Task RemoveAllSessionsByUserId(int userId);
}
