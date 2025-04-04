using Grpc.Core;
using User.Module.Repository.Interface;
using User.Module.Service.Interface;
namespace User.Module.Stubs
{
    public class UserServiceGrpcImpl : UserServiceGrpc.UserServiceGrpcBase
    {
        private readonly IUserService _service;
        private readonly IUserRepository _repository;

        public UserServiceGrpcImpl(IUserService service, IUserRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="conext"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<AuthUserResponse> GetUserByIdForAuth(UserRequest request, ServerCallContext conext)
        {

            var user = await this._service.FindUserById(request.Id);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            return new AuthUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                Roles = user.Roles,
                RefreshToken = user.RefreshToken ?? ""
            };
        }
        /// <summary>
        /// Update Refresh Token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<AuthUserResponse> UpdateRefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {

            var user = await this._service.UpdateRefreshToken(request.Id, request.RefreshToken ?? "");

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            return new AuthUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                Roles = user.Roles,
                RefreshToken = user.RefreshToken ?? ""
            };
        }

        /// <summary>
        ///  Find By Value 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> FindByValueForAuth(ValueKeysRequest request, ServerCallContext context)
        {

            var user = await this._repository.FindByKey(request.Key, request.StringValue);

            if (user == null)
                return new ValidationResponse
                {
                    Error = new ErrorResponse
                    {
                        StatusCode = 404,
                        Message = "User not found"
                    }
                };

            return new ValidationResponse
            {
                User = new AuthUserResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.Password,
                    Roles = user.Roles,
                    RefreshToken = user.RefreshToken ?? ""
                }
            };
        }

    }
}
