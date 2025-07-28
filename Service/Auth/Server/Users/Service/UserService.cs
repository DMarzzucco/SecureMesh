using Grpc.Core;
using Auth.Server.Users.Service.Interfaces;
using User;
using Auth.Server.Users.Helper;
using Auth.Server.Users.Maps;
using Auth.Server.Users.Model;
using Auth.Utils.Exceptions;

namespace Auth.Server.Users.Service
{
    public class UserService : IUserService
    {
        private readonly UserServiceGrpc.UserServiceGrpcClient _client;
        private readonly HandleGrpcError _handleGrpcError;
        private readonly RequestMapperUserGrpc _requestMapperUserGrpc;
        
        public UserService(UserServiceGrpc.UserServiceGrpcClient client, HandleGrpcError handleGrpcError, RequestMapperUserGrpc requestMapperUserGrpc)
        {
            _client = client;
            _handleGrpcError = handleGrpcError;
            this._requestMapperUserGrpc = requestMapperUserGrpc;
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
            try
            {
                var request = new UpdatePasswordDTORequest { Id = id, Password = body.OldPassword, NewPassword = body.NewPassword };
                var response = await this._client.UpdatePasswordAuthAsync(request);

                if (response.Error is not null && response.Error.StatusCode != 0)
                    this._handleGrpcError.InvokeError(response.Error);

                return response.Message;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"ERROR{ex.Message}");
                throw new Exception($"Error is {ex.Message}");
            }
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
            try
            {
                var response = await this._client.RegisterUserInAuthAsync(request);

                if (response.Error is not null && response.Error.StatusCode != 0)
                    this._handleGrpcError.InvokeError(response.Error);

                return this._requestMapperUserGrpc.InvokeValidationResponseMap(response);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"ERROR{ex.Message}");
                throw new Exception($"Error is {ex.Message}");
            }
        }
        /// <summary>
        /// Update Email Adress
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<UserModel> UpdateEmailAddress(int id, NewEmailDTO body)
        {
            var request = new NewEmailDTORequest
            {
                Id = id,
                Password = body.Password,
                NewEmail = body.NewEmail
            };
            var response = await this._client.UpdateEmailAdressAuthAsync(request);

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
            if (response.ResultCase == ValidationResponse.ResultOneofCase.Error)
                throw new KeyNotFoundException($"{response.Error.Message}");

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
            if (response.ResultCase == ValidationResponse.ResultOneofCase.Error)
                throw new KeyNotFoundException($"{response.Error.Message}");

            return this._requestMapperUserGrpc.InvokeValidationResponseMap(response);

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
            if (response.Error is not null && response.Error.StatusCode != 0)
                this._handleGrpcError.InvokeError(response.Error);

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

    }
}
