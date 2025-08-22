using System;
using IdentifyService.Server.UMS.Model;
using IdentifyService.Server.UMS.Helper;
using IdentifyService.Server.UMS.Maps;
using IdentifyService.Server.UMS.Model;
using IdentifyService.Server.UMS.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using UserManagementService.Proto.Server;

namespace IdentifyService.Server.UMS.Services;

public class ManagementUserFacedeServices : IManagementUserFacedeServices
{
    private readonly IdpFacedeService.IdpFacedeServiceClient _client;
    private readonly RequestMapperUserGrpc _requestMapper;
    private readonly HandleGrpcError _handleGrpcError;

    public ManagementUserFacedeServices(RequestMapperUserGrpc requestMapper, IdpFacedeService.IdpFacedeServiceClient client, HandleGrpcError handleGrpcError)
    {
        _requestMapper = requestMapper;
        _client = client;
        _handleGrpcError = handleGrpcError;
    }

    /// <summary>
    /// Registered User
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<UserModel> SaveUserRegistered(CreateUserDTO body)
    {
        var request = this._requestMapper.InvokeCreateUser(body);
        var response = await this._client.SaveUserRegisterAsync(request);
        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return this._requestMapper.InvokeValidationResponseMap(response);
    }

    /// <summary>
    /// Get User BY Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserModel> FindUserById(int id)
    {
        var request = new UserRequest { Id = id };
        var response = await this._client.GetUserByIdForAuthAsync(request);

        return this._requestMapper.InvokeUserModel(response);
    }

    /// <summary>
    /// Find by Username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<UserModel> FindByValue(string username)
    {
        var request = new UsernameRequest { Username = username };
        var response = await this._client.FindByUsernameAsync(request);
        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return this._requestMapper.InvokeValidationResponseMap(response);
    }

    /// <summary>
    /// Find User by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<UserModel> FindUserByEmail(string email)
    {
        var request = new EmailRequest { Email = email };
        var response = await this._client.GetUserByEmailAsync(request);

        var user = this._requestMapper.InvokeValidationResponseMap(response);
        return user;
    }

    /// <summary>
    /// Update User Password
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="sessionId"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<string> UpdatePasswordUser(int userId, int sessionId, UpdatePasswordDTO body)
    {
        var request = new UpdatePasswordRequest { UserId = userId, SessionId = sessionId, Password = body.OldPassword, NewPassword = body.NewPassword };

        var response = await this._client.UpdatePasswordUserAsync(request);
        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return response.Message;
    }

    /// <summary>
    /// Update User Email Address
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<UserModel> VerifyNewEmailParameters(int userId, NewEmailDTO body)
    {
        var request = new VerifyNewEmailRequest { UserId = userId, Password = body.Password, NewEmail = body.NewEmail };

        var response = await this._client.VerifyNewEmailAdressAsync(request);
        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return this._requestMapper.InvokeValidationResponseMap(response);
    }

    /// <summary>
    /// Get all list of sessions
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SessionModel>> FindAllSessionsByUserId(int userId)
    {
        var request = new UserRequest { Id = userId };
        var response = await this._client.GetAllUsersSessionsByUserIdAsync(request);
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
        });

        return session;
    }

    /// <summary>
    /// Find Session if exists
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public async Task<SessionModel> FindSessionIfExists(int userId, string ip, string userAgent, string location)
    {
        var request = new SessionRequest { UserId = userId, Ip = ip, UserAgent = userAgent, Location = location };
        var response = await this._client.FindSessionIfExistAsync(request);

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
        };

        return session;
    }

    /// <summary>
    /// Save Session Register
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public async Task<int> SaveSessionRegister(int userId, string ip, string userAgent, string location)
    {
        var request = new SessionRequest
        {
            UserId = userId,
            Ip = ip,
            UserAgent = userAgent,
            Location = location
        };

        var response = await this._client.SaveSessionAsync(request);

        return response.SessionId;
    }

    /// <summary>
    /// Remove session by id
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public async Task RemoveSessionById(int sessionId)
    {
        var request = new SessionIdRequest { SessionId = sessionId };
        await this._client.DeleteSessionByIdAsync(request);
    }

    /// <summary>
    /// Request to Remove Own Account
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task RequestToRemoveOwnAccount(int userId)
    {
        var request = new UserRequest { Id = userId };
        await this._client.AccountDelationRequestAsync(request);
    }

    /// <summary>
    /// Cancel Remove Account Operation if on
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task CancelRemoveAccountOperationIfOn(int userId)
    {
        var request = new UserRequest { Id = userId };
        await this._client.CancelAccountDelationOperationAsync(request);
    }
}
