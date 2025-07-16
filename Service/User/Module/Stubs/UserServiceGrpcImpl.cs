using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using User.Module.DTOs;
using User.Module.Repository.Interface;
using User.Module.Service.Interface;
using User.Module.Stubs.Handlers;
using User.Module.Stubs.Maps;
using User.Server.Interfaces;
using User.Utils.Exceptions;
namespace User.Module.Stubs
{
    public class UserServiceGrpcImpl : UserServiceGrpc.UserServiceGrpcBase
    {
        private readonly IUserService _service;
        private readonly IUserRepository _repository;
        private readonly MapResponseGrpc _mapper;
        private readonly HandlerGrpcExceptions _handlerGrpcExceptions;
        private readonly IHangFireServices hangFireServices;

        public UserServiceGrpcImpl(IUserService service, IUserRepository repository, MapResponseGrpc mapper, HandlerGrpcExceptions handlerGrpcExceptions, IHangFireServices hangFireServices)
        {
            this._service = service;
            this._repository = repository;
            this._mapper = mapper;
            this._handlerGrpcExceptions = handlerGrpcExceptions;
            this.hangFireServices = hangFireServices;
        }

        /// <summary>
        /// VerifyTwoAF
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<AuthUserResponse> VerifyTwoAF(VerifyTwoAFDTO request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.TwoAfCode))
                throw new RpcException(new Status(StatusCode.Cancelled, "The code not be empty"));

            var user = await this._repository.FindByEmailAsync(request.Email) ??
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            if (user.LockedAt != null && DateTime.UtcNow < user.LockedAt)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Account locked due to multiple failed attemps. Try again later"));

            if (user.TwoAFCode != request.TwoAfCode || user.TwoAFCodeExpiration < DateTime.UtcNow)
            {
                user.VerifyAttempts++;
                
                if (user.VerifyAttempts <= 3)
                {
                    user.LockedAt = DateTime.UtcNow.AddMinutes(10);
                    user.VerifyAttempts = 0;
                }
                await this._repository.UpdateAsync(user);
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Code is invalid or is expired"));
            }
            user.FirstLogin = false;
            user.TwoAFCode = Guid.NewGuid().ToString();
            user.TwoAFCodeExpiration = null;
            user.VerifyAttempts = 0;
            user.LockedAt = null;

            await this._repository.UpdateAsync(user);

            var response = this._mapper.InvokeMap(user);
            return response;
        }
        
        /// <summary>
        /// Update 2AF Code
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<Empty> UpdateTwoAFCode(TwoFADTO request, ServerCallContext context)
        {
            var user = await this._repository.FindByIdAsync(request.Id) ??
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            user.TwoAFCode = request.TwoAfCode;
            user.TwoAFCodeExpiration = request.TwoAfCodeExpiration.ToDateTime();
            
            await this._repository.UpdateAsync(user);

            return new Empty();
        }

        /// <summary>
        /// Cancelation Operation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<Empty> CancelationOperationAuth(UserRequest request, ServerCallContext context)
        {
            var user = await this._repository.FindByIdAsync(request.Id) ??
             throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            if (!user.IsDeleted)
                return new Empty();

            user.IsDeleted = false;
            user.DeletedAt = null;

            // this.backgroundJobClient.Delete(user.ScheduledDeletionJobId);
            this.hangFireServices.DeletedScheduledJob(user.ScheduledDeletionJobId);

            user.ScheduledDeletionJobId = null;

            await this._repository.UpdateAsync(user);

            return new Empty();
        }
        /// <summary>
        /// Deleted own account 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<MessageResponse> DeletedOwnAccountAuth(PasswordDTORequest request, ServerCallContext context)
        {
            try
            {
                var dto = new PasswordDTO { Password = request.Password };
                var reg = await this._service.RemoveUserRegisterForBasicRoles(request.Id, dto);

                return new MessageResponse { Message = reg };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeMessageResponse(ex); }
        }
        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> RegisterUserInAuth(CreateUserRequest request, ServerCallContext context)
        {
            try
            {
                var dto = new CreateUserDTO
                {
                    FullName = request.FullName,
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    Roles = request.Roles
                };
                var reg = await this._service.RegisterUser(dto);
                var response = this._mapper.InvokeMap(reg);


                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }
        }
        /// <summary>
        /// Update Email Adrres 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> UpdateEmailAdressAuth(NewEmailDTORequest request, ServerCallContext context)
        {
            try
            {
                var body = new NewEmailDTO { Password = request.Password, NewEmail = request.NewEmail };
                var reg = await this._service.UpdateEmail(request.Id, body);
                var response = this._mapper.InvokeMap(reg);

                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }
        }
        /// <summary>
        /// Get User by key
        /// </summary>
        /// <param name="request"></param>
        /// <param name="conext"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<ValidationResponse> GetUserByIdForAuth(UserRequest request, ServerCallContext conext)
        {
            try
            {
                var user = await this._service.FindUserById(request.Id) ??
                    throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

                var response = this._mapper.InvokeMap(user);

                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }

        }
        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> GetUserByEmailForAuth(UserEmailRequest request, ServerCallContext context)
        {
            try
            {
                var user = await this._service.GetUserByEmail(request.Email);

                var response = this._mapper.InvokeMap(user);

                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }
        }
        /// <summary>
        /// Return Password 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> ReturnPasswordForAuth(PasswordDTORequest request, ServerCallContext context)
        {
            try
            {
                var body = new PasswordDTO { Password = request.Password };

                var reg = await this._service.ReturnPasswordAsync(request.Id, body);
                var response = this._mapper.InvokeMap(reg);

                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }
        }
        /// <summary>
        /// Update Refresh Token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> UpdateRefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            try
            {
                var user = await this._service.UpdateRefreshToken(request.Id, request.RefreshToken ?? "");
                var response = this._mapper.InvokeMap(user);
                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }
        }

        /// <summary>
        /// Marl Email Verify
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<ValidationResponse> MarkEmailVerifyAuth(UserRequest request, ServerCallContext context)
        {
            try
            {
                var user = await this._service.MarkEmailAsVerifieds(request.Id);

                var response = this._mapper.InvokeMap(user);
                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }
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
            var response = this._mapper.InvokeMap(user);

            return new ValidationResponse { User = response };
        }

    }
}
