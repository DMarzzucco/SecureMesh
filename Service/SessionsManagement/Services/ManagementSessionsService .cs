using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SessionsManagement.Module.Model;
using SessionsManagement.Module.Repository.Interfaces;
using SessionsManagement.Protos;

namespace SessionsManagement.Services;

public class ManagementSessionsServices(IManagementSessionsRepository repository) : SessionsManagementServiceGrpc.SessionsManagementServiceGrpcBase

{
    private readonly IManagementSessionsRepository repository = repository;

    /// <summary>
    /// Remove all sessions by user Id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<Empty> DeleteAllSessionsByUserId(UserIdRequest request, ServerCallContext context)
    {
        await this.repository.RemoveAllSessionsByUserId(request.UserId);
        
        return new Empty();
    }

    /// <summary>
    /// Remove all session except current
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<Empty> DeleteAllSessionsExceptCurrent(DeleteAllSessionExceptCurrentRequest request, ServerCallContext context)
    {
        await this.repository.RemoveAllSessionsExceptCurrent(request.UserId, request.CurrentSessionId);
        return new Empty();
    }

    /// <summary>
    /// Find Sessions by props
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<FindResponse> FindSessionByProps(SessionsPropsRequest request, ServerCallContext context)
    {
        var session = await this.repository.FindSession(request.UserId, request.Ip, request.UserAgent, request.Location);

        if (session == null) return new FindResponse
        {
            Reason = new NotFoundResponse { Reason = "Not found session in this user" }
        };

        var response = new SessionResponse
        {
            Id = session.Id,
            UserId = session.UserId,
            Ip = session.Ip,
            UserAgent = session.UserAgent,
            Location = session.Location,
            IsActive = session.IsActive
        };

        return new FindResponse { Session = response };
    }

    /// <summary>
    /// Remove session by Id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<Empty> DeleteSessionById(IdRequest request, ServerCallContext context)
    {
        var session = await this.repository.FindSessionById(request.Id) ??
            throw new RpcException(new Status(StatusCode.NotFound, "Session not found"));

        await this.repository.RemoveSessionAsync(session);

        return new Empty();
    }

    /// <summary>
    /// Find session by user id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<FindListResponse> FindListSessionsByUser(UserIdRequest request, ServerCallContext context)
    {
        var session = await this.repository.FindAllSessionsByUserId(request.UserId);

        if (session == null) return new FindListResponse
        {
            Reason = new NotFoundResponse { Reason = "Not found session in this user" }
        };

        var response = new FindListResponse();
        response.Sessions.AddRange(session.Select(s => new SessionResponse
        {
            Id = s.Id,
            UserId = s.UserId,
            Ip = s.Ip,
            UserAgent = s.UserAgent,
            Location = s.Location,
            IsActive = s.IsActive
        }));

        return response;
    }

    /// <summary>
    /// Save Session
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task<IdResponse> SaveSession(SessionsPropsRequest request, ServerCallContext context)
    {
        var session = new SessionModel
        {
            UserId = request.UserId,
            Ip = request.Ip,
            UserAgent = request.UserAgent,
            Location = request.Location,
            IsActive = true
        };

        await this.repository.SaveSession(session);

        return new IdResponse { Id = session.Id };
    }
}
