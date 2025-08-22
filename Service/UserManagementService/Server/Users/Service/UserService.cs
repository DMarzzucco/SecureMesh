using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using User;
using UserManagementService.Server.Users.Helper;
using UserManagementService.Server.Users.Maps;
using UserManagementService.Server.Users.Model;
using UserManagementService.Server.Users.Service.Interfaces;
using UserManagementService.Utils.Exceptions;

namespace UserManagementService.Server.Users.Service;

public class UserService : IUserService
{
    private readonly UserServiceGrpc.UserServiceGrpcClient _client;
    private readonly HandleGrpcError _handleGrpcError;
    private readonly RequestMapperUserGrpc _requestMapperUserGrpc;

    private readonly IMapper _mapper;

    public UserService(UserServiceGrpc.UserServiceGrpcClient client, HandleGrpcError handleGrpcError, RequestMapperUserGrpc requestMapperUserGrpc, IMapper mapper)
    {
        _client = client;
        _handleGrpcError = handleGrpcError;
        _requestMapperUserGrpc = requestMapperUserGrpc;
        _mapper = mapper;
    }

    /// <summary>
    /// Update Roles
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<string> UpdateUserRoles(int id, RolesDTO body)
    {
        var request = new UpdateRolesRequest { Id = id, NewRoles = (User.ROLES)body.NewRoles };
        var response = await this._client.UpdateRolesUserAsync(request);

        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return response.Message;
    }

    /// <summary>
    /// Update Own Registered
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<string> UpdateOwnRegister(int id, UpdateOwnRegisterDTO body)
    {
        var request = new UpdateOwnUserDTORequest { Id = id, Password = body.Password, FullName = body.FullName, Username = body.Username };

        var response = await this._client.UpdateOwnAccountAsync(request);

        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return response.Message;
    }

    /// <summary>
    /// List of All Users
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<UserDTO>> ListOfAllUsers()
    {
        var empty = new Empty();
        var list = await this._client.GetListOfAllUsersAsync(empty);

        var response = list.User.Select(u => new UserModel
        {
            Id = u.Id,
            FullName = u.FullName,
            Username = u.Username,
            Email = u.Email,
            Password = u.Password,
            Roles = (Model.ROLES)u.Roles
        });

        var listUser = this._mapper.Map<IEnumerable<UserDTO>>(response);

        return listUser;
    }

    /// <summary>
    /// Get user Profile
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserDTO> GetUserProfile(int id)
    {
        var request = new UserRequest { Id = id };
        var responseClient = await this._client.GetUserByIdForAuthAsync(request);

        var user = this._requestMapperUserGrpc.InvokeUserModel(responseClient);

        return this._mapper.Map<UserDTO>(user);
    }

    /// <summary>
    /// Update password
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> UpdatePasswordUser(int id, UpdatePasswordDTO body)
    {
        var request = new UpdatePasswordDTORequest { Id = id, Password = body.OldPassword, NewPassword = body.NewPassword };
        var response = await this._client.UpdatePasswordAuthAsync(request);

        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return response.Message;
    }

    /// <summary>
    /// Deleted own account 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task RemoveUser(int id)
    {
        try
        {
            var request = new UserRequest { Id = id };

            await this._client.DeleteAccountAsync(request);
        }
        catch (NotFoundExceptions ex) { throw new NotFoundExceptions(ex.Message); }
    }

    /// <summary>
    /// Registered User
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<UserModel> RegisterUser(CreateUserDTO body)
    {
        var request = new CreateUserRequest
        {
            FullName = body.FullName,
            Username = body.Username,
            Email = body.Email,
            Password = body.Password,
            Roles = (User.ROLES)body.Roles
        };
        var response = await this._client.RegisterUserInAuthAsync(request);

        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return this._requestMapperUserGrpc.InvokeValidationResponseMap(response);
    }

    /// <summary>
    /// Verify New Email Address Parameters
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<UserModel> VerifyNewEmailAdressParameters(int id, NewEmailDTO body)
    {
        var request = new NewEmailDTORequest
        {
            Id = id,
            Password = body.Password,
            NewEmail = body.NewEmail
        };
        var response = await this._client.VerifyNewEmailParametersAsync(request);
        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return this._requestMapperUserGrpc.InvokeValidationResponseMap(response);
    }
    /// <summary>
    /// Update Email Address
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newEmail"></param>
    /// <returns></returns>
    public async Task<UserModel> UpdateEmailAddress(int id, string newEmail)
    {
        var request = new UpdateEmailRequest { Id = id, NewEmail = newEmail };
        var response = await this._client.UpdateEmailAddressAsync(request);
        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return this._requestMapperUserGrpc.InvokeValidationResponseMap(response);
    }

    /// <summary>
    /// Find By Value for validate credentials
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<UserModel> FindByValue(string key, object value)
    {
        var request = new ValueKeysRequest { Key = key, StringValue = value.ToString() };

        var response = await this._client.FindByValueForAuthAsync(request);
        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return this._requestMapperUserGrpc.InvokeValidationResponseMap(response);
    }

    /// <summary>
    /// Get User by key value
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<UserModel> GetUserById(int id)
    {
        var request = new UserRequest { Id = id };
        var response = await this._client.GetUserByIdForAuthAsync(request);

        return this._requestMapperUserGrpc.InvokeUserModel(response);
    }
    /// <summary>
    /// Get User By Email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<UserModel> GetUserByEmail(string email)
    {
        var request = new UserEmailRequest { Email = email };

        var response = await this._client.GetUserByEmailForAuthAsync(request);
        
        return this._requestMapperUserGrpc.InvokeValidationResponseMap(response);

    }
    /// <summary>
    /// Return Password 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<UserModel> ReturnPassword(int id, PasswordDTO body)
    {
        var request = new PasswordDTORequest { Id = id, Password = body.Password };

        var response = await this._client.ReturnPasswordForAuthAsync(request);
        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return this._requestMapperUserGrpc.InvokeValidationResponseMap(response);
    }

    /// <summary>
    /// Remove Any Account
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string> RemoveAnyAccount(int id)
    {
        var request = new UserRequest { Id = id };
        var response = await this._client.DeleteAnyAccountAsync(request);

        if (response.Error is not null && response.Error.StatusCode != 0)
            this._handleGrpcError.InvokeError(response.Error);

        return response.Message;
    }
}
