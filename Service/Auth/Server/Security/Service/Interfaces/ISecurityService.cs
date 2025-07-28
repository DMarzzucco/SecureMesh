using System;
using Auth.Server.Security.Model;

namespace Auth.Server.Security.Service.Interfaces;

public interface ISecurityService
{
    Task RemoveSessionById(int id);
    Task<SessionModel?> SessionExist(int userId, string ip, string userAgent, string location);
    Task<IEnumerable<SessionModel?>> FindAllSessionsByUserId(int userId);
    Task<int> SaveSession(int userId, string ip, string userAgent, string location);
    Task RemoveAllSessionExceptCurrent(int userId, int currentSessionId);
    Task RemoveAllSessionsByUserId(int userId);
}
