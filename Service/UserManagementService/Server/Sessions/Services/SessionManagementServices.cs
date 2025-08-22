using SessionsManagement.Protos;
using UserManagementService.Server.Sessions.Models;
using UserManagementService.Server.Sessions.Services.Interfaces;

namespace UserManagementService.Server.Sessions.Services;

public class SessionManagementServices(SessionsManagementServiceGrpc.SessionsManagementServiceGrpcClient client) : ISessionManagementServices
{
    private readonly SessionsManagementServiceGrpc.SessionsManagementServiceGrpcClient client = client;

    /// <summary>
    /// Remove all sessions of user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task RemoveAllSessionsByUserId(int userId)
    {
        var request = new UserIdRequest { UserId = userId };
        await this.client.DeleteAllSessionsByUserIdAsync(request);
    }

    /// <summary>
    /// Remove all sessions except current
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="currentSessionId"></param>
    /// <returns></returns>
    public async Task RemoveAllSessionExceptCurrent(int userId, int currentSessionId)
    {
        var request = new DeleteAllSessionExceptCurrentRequest { UserId = userId, CurrentSessionId = currentSessionId };
        await this.client.DeleteAllSessionsExceptCurrentAsync(request);
    }
    /// <summary>
    /// Remove session by id 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task RemoveSessionById(int id)
    {
        var request = new IdRequest { Id = id };
        await this.client.DeleteSessionByIdAsync(request);
    }

    /// <summary>
    /// Find Session if this exist
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public async Task<SessionsModel?> SessionExist(int userId, string ip, string userAgent, string location)
    {
        var request = new SessionsPropsRequest { UserId = userId, Ip = ip, UserAgent = userAgent, Location = location };

        var response = await this.client.FindSessionByPropsAsync(request);

        if (response.Session == null)
        {
            Console.WriteLine(response.Reason);
            return null;
        }
        var session = new SessionsModel
        {
            Id = response.Session.Id,
            UserId = response.Session.UserId,
            Ip = response.Session.Ip,
            UserAgent = response.Session.UserAgent,
            Location = response.Session.Location,
        };

        return session;
    }
    /// <summary>
    /// Find Session by user Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SessionsModel?>> FindAllSessionsByUserId(int userId)
    {
        var request = new UserIdRequest { UserId = userId };
        var response = await this.client.FindListSessionsByUserAsync(request);

        if (response.Sessions == null || response.Sessions.Count == 0)
        {
            Console.WriteLine(response.Reason);
            return [];
        }

        var session = response.Sessions.Select(s => new SessionsModel
        {
            Id = s.Id,
            UserId = s.UserId,
            Ip = s.Ip,
            UserAgent = s.UserAgent,
            Location = s.Location,
        });

        return session;
    }

    /// <summary>
    /// Save session 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<int> SaveSessionRegister(int userId, string ip, string userAgent, string location)
    {
        var request = new SessionsPropsRequest
        {
            UserId = userId,
            Ip = ip,
            UserAgent = userAgent,
            Location = location
        };

        var response = await this.client.SaveSessionAsync(request);

        return response.Id;
    }
}
