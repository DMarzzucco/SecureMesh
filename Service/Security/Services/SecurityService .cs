using System.Reflection.Metadata.Ecma335;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Security;
using Security.Module.Model;
using Security.Module.Repository.Interfaces;
using Security.Protos;

namespace Security.Services;

public class SecurityService : SecurityServiceGrpc.SecurityServiceGrpcBase
{
    private readonly ISecurityRepository repository;
    private readonly IHttpContextAccessor httpContext;

    public SecurityService(ISecurityRepository repository, IHttpContextAccessor httpContext)
    {
        this.repository = repository;
        this.httpContext = httpContext;
    }

    /// <summary>
    /// Find session by user id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<SessionResponse> FindSessionByUser(UserIdRequest request, ServerCallContext context)
    {
        var session = await this.repository.FindByUserId(request.UserId);

        var response = new SessionResponse
        {
            Id = session.Id,
            UserId = session.UserId,
            Ip = session.Ip,
            UserAgent = session.UserAgent,
            Location = session.Location,
            IsActive = session.IsActive
        };
        return response;
    }
    
    /// <summary>
    /// Save Session
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task<Empty> SaveSession(UserIdRequest request, ServerCallContext context)
    {
        var httpContexts = this.httpContext.HttpContext ??
            throw new Exception();

        var session = new SecurityModel
        {
            UserId = request.UserId,
            Ip = httpContexts.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContexts.Request.Headers.UserAgent.ToString(),
            Location = ""
        };

        await this.repository.SaveSession(session);

        return new Empty();
    }
}
