using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using User.Module.DTOs;
using User.Module.Repository.Interface;
using User.Module.Service.Interface;
using User.Module.Stubs.Handlers;
using User.Module.Stubs.Maps;
namespace User.Module.Stubs
{
    public class UserServiceGrpcImpl : UserServiceGrpc.UserServiceGrpcBase
    {
        private readonly IUserService _service;
        private readonly IUserRepository _repository;
        private readonly MapResponseGrpc _mapper;
        private readonly HandlerGrpcExceptions _handlerGrpcExceptions;

        public UserServiceGrpcImpl(IUserService service, IUserRepository repository, MapResponseGrpc mapper, HandlerGrpcExceptions handlerGrpcExceptions)
        {
            this._service = service;
            this._repository = repository;
            this._mapper = mapper;
            this._handlerGrpcExceptions = handlerGrpcExceptions;
        }

        /// <summary>
        /// Remove Any Account 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<MessageResponse> DeleteAnyAccount(UserRequest request, ServerCallContext context)
        {
            try
            {
                var op = await this._service.RemoveUserRegister(request.Id);
                var response = new MessageResponse { Message = op };
                return response;
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeMessageResponse(ex); }
        }

        /// <summary>
        /// List of all users
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ListOfUserResponse> GetListOfAllUsers(Empty request, ServerCallContext context)
        {
            var list = await this._repository.ToListAsync();

            var response = new ListOfUserResponse();

            response.User.AddRange(list.Select(u => new AuthUserResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Username = u.Username,
                Email = u.Email,
                Password = u.Password,
                Roles = u.Roles
            }));

            return response;
        }

        /// <summary>
        /// Update User Roles
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<MessageResponse> UpdateRolesUser(UpdateRolesRequest request, ServerCallContext context)
        {
            try
            {
                var reg = await this._service.UpdateRoles(request.Id, request.NewRoles);
                var response = new MessageResponse { Message = reg };

                return response;
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeMessageResponse(ex); }
        }

        /// <summary>
        /// Update Own Account
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<MessageResponse> UpdateOwnAccount(UpdateOwnUserDTORequest request, ServerCallContext context)
        {
            try
            {
                var dto = new UpdateOwnUserDTO { Password = request.Password, FullName = request.FullName, Username = request.Username };

                var reg = await this._service.UpdateOwnRegister(request.Id, dto);
                var response = new MessageResponse { Message = reg };

                return response;
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeMessageResponse(ex); }
        }

        /// <summary>
        /// Remove Account
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override async Task<Empty> DeleteAccount(UserRequest request, ServerCallContext context)
        {
            var user = await this._repository.FindByIdAsync(request.Id) ??
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            await this._repository.DeleteAsync(user);

            return new Empty();
        }
        /// <summary>
        /// Update Password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<MessageResponse> UpdatePasswordAuth(UpdatePasswordDTORequest request, ServerCallContext context)
        {
            try
            {
                var reg = await this._service.UpdatePassword(request.Id, request.Password, request.NewPassword);
                var response = new MessageResponse { Message = reg };

                return response;
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
        /// Validate New Email Adrres  Parameters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> VerifyNewEmailParameters(NewEmailDTORequest request, ServerCallContext context)
        {
            try
            {
                var reg = await this._service.VerifyNewEmailParameters(request.Id, request.Password, request.NewEmail);
                var response = this._mapper.InvokeMap(reg);

                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }
        }
        /// <summary>
        /// Update Email
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> UpdateEmailAddress(UpdateEmailRequest request, ServerCallContext context)
        {
            try
            {
                var user = await this._service.UpdateEmail(request.Id, request.NewEmail);
                var response = this._mapper.InvokeMap(user);
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
        public override async Task<AuthUserResponse> GetUserByIdForAuth(UserRequest request, ServerCallContext conext)
        {
            var user = await this._repository.FindByIdAsync(request.Id) ??
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            var response = this._mapper.InvokeMap(user);

            return response;
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ValidationResponse> GetUserByEmailForAuth(UserEmailRequest request, ServerCallContext context)
        {
            var user = await this._repository.FindByEmailAsync(request.Email) ??
                throw new RpcException(new Status(StatusCode.NotFound, "User was not found"));

            var response = this._mapper.InvokeMap(user);

            return new ValidationResponse { User = response };
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
                var reg = await this._service.ReturnPasswordAsync(request.Id, request.Password);
                var response = this._mapper.InvokeMap(reg);

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
            try
            {
                var user = await this._service.FindValueByKey(request.Key, request.StringValue);
                var response = this._mapper.InvokeMap(user);

                return new ValidationResponse { User = response };
            }
            catch (Exception ex) { return this._handlerGrpcExceptions.InvokeExceptions(ex); }
        }

    }
}
