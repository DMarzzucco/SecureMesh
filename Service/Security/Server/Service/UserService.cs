using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Security.Server.Helper;
using Security.Server.Maps;
using Security.Server.Model;
using Security.Server.Service.Interfaces;
using User;

namespace Security.Server.Service
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
        /// UpdateCsrfToken
        /// </summary>
        /// <param name="id"></param>
        /// <param name="csrfToken"></param>
        /// <param name="csrfTokenExpiration"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateCsrfToken(int id, string csrfToken, DateTime csrfTokenExpiration)
        {
            var request = new CsrfTokenRequest { Id = id, CsrfToken = csrfToken, CsrfTokenExpiration = csrfTokenExpiration.ToTimestamp() };
            try
            {
                await this._client.UpdateCsrfTokenAuthAsync(request);
            }
            catch (Exception ex) { throw new Exception($"{ex.Message}"); }
        }
        /// <summary>
        /// Deleted own account 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<string> DeletedOwnAccount(int id, PasswordDTO dto)
        {
            var request = new PasswordDTORequest { Id = id, Password = dto.Password };

            var response = await this._client.DeletedOwnAccountAuthAsync(request);

            if (response.Error is not null && response.Error.StatusCode != 0)
                this._handleGrpcError.InvokeError(response.Error);

            return response.Message;
        }
        /// <summary>
        /// Cancelation operation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task CancelationOperation(int id)
        {
            var request = new UserRequest { Id = id };
            try
            {
                await this._client.CancelationOperationAuthAsync(request);
            }
            catch (Exception ex) { throw new Exception($"{ex.Message}"); }
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

                return this._requestMapperUserGrpc.InvokeMap(response);
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
        public async Task<UserModel> UpdateEmailAdress(int id, NewEmailDTO body)
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

            return this._requestMapperUserGrpc.InvokeMap(response);
        }
        /// <summary>
        /// Mark Email 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserModel> MarkEmailAsync(int id)
        {
            var request = new UserRequest { Id = id };
            var response = await this._client.MarkEmailVerifyAuthAsync(request);
            if (response.Error is not null && response.Error.StatusCode != 0)
                this._handleGrpcError.InvokeError(response.Error);

            return this._requestMapperUserGrpc.InvokeMap(response);
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

            return this._requestMapperUserGrpc.InvokeMap(response);
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

            return this._requestMapperUserGrpc.InvokeMap(response);

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

            return this._requestMapperUserGrpc.InvokeMap(response);

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

            return this._requestMapperUserGrpc.InvokeMap(response);
        }
        /// <summary>
        /// Update Refresh Token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task UpdateRefreshToken(int id, string? refreshToken)
        {
            var request = new RefreshTokenRequest { Id = id, RefreshToken = refreshToken ?? "" };
            try
            {
                await this._client.UpdateRefreshTokenAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error to calling gRPC Server{ex.Message}");
            }
        }
    }
}
