using System;
using UserManagementService.Server.Sessions.Models;

namespace UserManagementService.Server.Sessions.Services.Interfaces;

public interface ISessionManagementServices
{
    Task RemoveSessionById(int id);
    Task<SessionsModel?> SessionExist(int userId, string ip, string userAgent, string location);
    Task<IEnumerable<SessionsModel?>> FindAllSessionsByUserId(int userId);
    Task<int> SaveSessionRegister(int userId, string ip, string userAgent, string location);
    Task RemoveAllSessionExceptCurrent(int userId, int currentSessionId);
    Task RemoveAllSessionsByUserId(int userId);
}
