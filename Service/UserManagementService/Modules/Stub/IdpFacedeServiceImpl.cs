using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UserManagementService.Modules.Repository.Interfaces;
using UserManagementService.Modules.Stub.Helper;
using UserManagementService.Proto.Server;
using UserManagementService.Server.Hangfire.Services.Interfaces;
using UserManagementService.Server.Sessions.Models;
using UserManagementService.Server.Sessions.Services.Interfaces;
using UserManagementService.Server.Users.Model;
using UserManagementService.Server.Users.Service.Interfaces;

namespace UserManagementService.Modules.Stub;

public class IdpFacedeServiceImpl : IdpFacedeService.IdpFacedeServiceBase
{
    private readonly IUserService _userService;
    private readonly ISessionManagementServices _sessionService;
    private readonly IManagementUserRepository _repository;
    private readonly MapModelsGrpc _mapgRPC;
    private readonly MapGrpcExceptions _mapgRPCExcetions;
    private readonly IHangFireService _hangFireService;

    public IdpFacedeServiceImpl(IUserService userService, ISessionManagementServices sessionService, IManagementUserRepository repository, MapModelsGrpc mapgRPC, MapGrpcExceptions mapgRPCExcetions, IHangFireService hangFireService)
    {
        _userService = userService;
        _sessionService = sessionService;
        _repository = repository;
        _mapgRPC = mapgRPC;
        _mapgRPCExcetions = mapgRPCExcetions;
        _hangFireService = hangFireService;
    }

    /// <summary>
    /// Get user by Id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<UserResponse> GetUserByIdForAuth(UserRequest request, ServerCallContext context)
    {
        var user = await this._userService.GetUserById(request.Id);
        return this._mapgRPC.UserModelMapper(user);
    }

    /// <summary>
    /// Registered User
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<MultipleUserResponse> SaveUserRegister(CreateUserRequest request, ServerCallContext context)
    {
        var body = this._mapgRPC.CreateUserMapper(request);
        try
        {
            var user = await this._userService.RegisterUser(body);

            await this._repository.SaveRelationManagementByUserId(user.Id);

            var response = this._mapgRPC.UserModelMapper(user);
            return new MultipleUserResponse { User = response };
        }
        catch (Exception ex) { return this._mapgRPCExcetions.InvokeExceptions(ex); }
    }
    /// <summary>
    /// Update Password
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<MessageResponse> UpdatePasswordUser(UpdatePasswordRequest request, ServerCallContext context)
    {
        try
        {
            var dto = new UpdatePasswordDTO { OldPassword = request.Password, NewPassword = request.NewPassword };
            var response = await this._userService.UpdatePasswordUser(request.UserId, dto);

            await this._sessionService.RemoveAllSessionExceptCurrent(request.UserId, request.SessionId);

            return new MessageResponse { Message = response };
        }
        catch (Exception ex) { return this._mapgRPCExcetions.InvokeMessageResponse(ex); }
    }

    /// <summary>
    /// Update Email Address
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<MultipleUserResponse> VerifyNewEmailAdress(VerifyNewEmailRequest request, ServerCallContext context)
    {
        try
        {
            var dto = new NewEmailDTO { Password = request.Password, NewEmail = request.NewEmail };
            var user = await this._userService.VerifyNewEmailAdressParameters(request.UserId, dto);

            return new MultipleUserResponse { User = this._mapgRPC.UserModelMapper(user) };
        }
        catch (Exception ex) { return this._mapgRPCExcetions.InvokeExceptions(ex); }
    }

    /// <summary>
    /// Find By Username
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<MultipleUserResponse> FindByUsername(UsernameRequest request, ServerCallContext context)
    {
        try
        {
            var user = await this._userService.FindByValue("Username", request.Username);
            return new MultipleUserResponse { User = this._mapgRPC.UserModelMapper(user) };
        }
        catch (Exception ex) { return this._mapgRPCExcetions.InvokeExceptions(ex); }
    }

    /// <summary>
    /// Get User By Email
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<MultipleUserResponse> GetUserByEmail(EmailRequest request, ServerCallContext context)
    {
        var user = await this._userService.GetUserByEmail(request.Email) ??
            throw new RpcException(new Status (StatusCode.NotFound, "User was not found"));
        return new MultipleUserResponse { User = this._mapgRPC.UserModelMapper(user) };
    }

    /// <summary>
    /// Get all list of sessions user by user id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<GetSessionsListResponse> GetAllUsersSessionsByUserId(UserRequest request, ServerCallContext context)
    {
        var session = await this._sessionService.FindAllSessionsByUserId(request.Id);

        if (session == null) return new GetSessionsListResponse
        {
            Reason = new NotFoundResponse { Reason = "Not found session in this user" }
        };

        var response = new GetSessionsListResponse();
        response.Sessions.AddRange(session.Select(s => new SessionResponse
        {
            Id = s.Id,
            UserId = s.UserId,
            Ip = s.Ip,
            UserAgent = s.UserAgent,
            Location = s.Location,
        }));

        return response;

    }

    /// <summary>
    /// Find session if exist
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<FindSessionResponse> FindSessionIfExist(SessionRequest request, ServerCallContext context)
    {
        var session = await this._sessionService.SessionExist(request.UserId, request.Ip, request.UserAgent, request.Location);

        if (session == null) return new FindSessionResponse
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
        };

        return new FindSessionResponse { Session = response };
    }

    /// <summary>
    /// Save Session
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<SessionIdResponse> SaveSession(SessionRequest request, ServerCallContext context)
    {

        int id = await this._sessionService.SaveSessionRegister(request.UserId, request.Ip, request.UserAgent, request.Location);

        return new SessionIdResponse { SessionId = id };
    }

    /// <summary>
    /// Remove Session by id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<Empty> DeleteSessionById(SessionIdRequest request, ServerCallContext context)
    {
        await this._sessionService.RemoveSessionById(request.SessionId);
        return new Empty();
    }

    /// <summary>
    /// Account Delation Request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<Empty> AccountDelationRequest(UserRequest request, ServerCallContext context)
    {
        var ms = await this._repository.GetRelationManagementByUserId(request.Id) ??
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        ms.IsDeleted = true;
        ms.DeletedAt = DateTime.UtcNow;
        ms.ScheduledDeletionJobId = this._hangFireService.ScheduleIdKey(request.Id);

        await this._repository.UpdateManagementUser(ms);

        return new Empty();
    }

    /// <summary>
    /// Cancel Account Delation Operation If On
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<Empty> CancelAccountDelationOperation(UserRequest request, ServerCallContext context)
    {
        var ms = await this._repository.GetRelationManagementByUserId(request.Id) ??
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        if (ms.IsDeleted)
        {
            ms.IsDeleted = false;
            ms.DeletedAt = null;

            this._hangFireService.DeletedScheduledJob(ms.ScheduledDeletionJobId);
            ms.ScheduledDeletionJobId = null;

            await this._repository.UpdateManagementUser(ms);
        }

        return new Empty();
    }
}
