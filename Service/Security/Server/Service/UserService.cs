using Grpc.Core;
using Security.Server.Model;
using Security.Server.Service.Interfaces;
using User;

namespace Security.Server.Service
{
    public class UserService : IUserService
    {
        private readonly UserServiceGrpc.UserServiceGrpcClient _client;

        public UserService(UserServiceGrpc.UserServiceGrpcClient client)
        {
            _client = client;
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
            try
            {
                var request = new ValueKeysRequest { Key = key, StringValue = value.ToString() };

                var response = await this._client.FindByValueForAuthAsync(request);

                if (response.ResultCase == ValidationResponse.ResultOneofCase.Error)
                    throw new KeyNotFoundException($"{response.Error.Message}");

                var user = new UserModel
                {
                    Id = response.User.Id,
                    FullName = response.User.FullName,
                    Username = response.User.Username,
                    Email = response.User.Email,
                    Password = response.User.Password,
                    Roles = (Model.ROLES)response.User.Roles,
                    RefreshToken = response.User.RefreshToken
                };
                return user;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"ERROR{ex.Message}");
                throw new Exception($"Error {ex.Message}");
            }
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
            try
            {
                var response = await this._client.GetUserByIdForAuthAsync(request);

                var user = new UserModel
                {
                    Id = response.Id,
                    FullName = response.FullName,
                    Username = response.Username,
                    Email = response.Email,
                    Password = response.Password,
                    Roles = (Model.ROLES)response.Roles,
                    RefreshToken = response.RefreshToken
                };
                return user;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error to calling gRPC Server{ex.Message}");
            }
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
