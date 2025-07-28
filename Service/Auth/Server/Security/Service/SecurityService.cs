using System;
using Auth.Server.Security.Model;
using Auth.Server.Security.Service.Interfaces;
using Auth.Utils.Exceptions;
using Security.Protos;

namespace Auth.Server.Security.Service;

public class SecurityService(SecurityServiceGrpc.SecurityServiceGrpcClient client) : ISecurityService
{
    private readonly SecurityServiceGrpc.SecurityServiceGrpcClient client = client;

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
        try
        {
            var request = new IdRequest { Id = id };
            await this.client.DeleteSessionByIdAsync(request);
        }
        catch (NotFoundExceptions ex)
        {
            throw new NotFoundExceptions($"{ex.Message}");
        }
    }

    /// <summary>
    /// Find Session if this exist
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public async Task<SessionModel?> SessionExist(int userId, string ip, string userAgent, string location)
    {
        var request = new SessionsPropsRequest { UserId = userId, Ip = ip, UserAgent = userAgent, Location = location };

        var response = await this.client.FindSessionByPropsAsync(request);

        if (response.Session == null)
        {
            Console.WriteLine(response.Reason);
            return null;
        }
        var session = new SessionModel
        {
            Id = response.Session.Id,
            UserId = response.Session.UserId,
            Ip = response.Session.Ip,
            UserAgent = response.Session.UserAgent,
            Location = response.Session.Location,
            IsActive = response.Session.IsActive
        };

        return session;
    }
    /// <summary>
    /// Find Session by user Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SessionModel?>> FindAllSessionsByUserId(int userId)
    {
        var request = new UserIdRequest { UserId = userId };
        var response = await this.client.FindListSessionsByUserAsync(request);

        if (response.Sessions == null || response.Sessions.Count == 0)
        {
            Console.WriteLine(response.Reason);
            return [];
        }

        var session = response.Sessions.Select(s => new SessionModel
        {
            Id = s.Id,
            UserId = s.UserId,
            Ip = s.Ip,
            UserAgent = s.UserAgent,
            Location = s.Location,
            IsActive = s.IsActive
        });

        return session;
    }

    /// <summary>
    /// Save session 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<int> SaveSession(int userId, string ip, string userAgent, string location)
    {
        var request = new SaveSessionRequest
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
