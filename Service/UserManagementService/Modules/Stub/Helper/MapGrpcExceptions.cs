using System;
using UserManagementService.Proto.Server;
using UserManagementService.Utils.Exceptions;

namespace UserManagementService.Modules.Stub.Helper;

public class MapGrpcExceptions
{
    public MultipleUserResponse InvokeExceptions(Exception ex)
        {
            return ex switch
            {
                BadRequestExceptions e => new MultipleUserResponse { Error = new ErrorResponse { StatusCode = 400, Message = e.Message } },
                ForbiddenExceptions e => new MultipleUserResponse { Error = new ErrorResponse { StatusCode = 403, Message = e.Message } },
                NotFoundExceptions e => new MultipleUserResponse { Error = new ErrorResponse { StatusCode = 404, Message = e.Message } },
                ConflictExceptions e => new MultipleUserResponse { Error = new ErrorResponse { StatusCode = 409, Message = e.Message } },
                _ => new MultipleUserResponse { Error = new ErrorResponse { StatusCode = 500, Message = "Unexpected error: " + ex.Message } }
            };
        }
        public MessageResponse InvokeMessageResponse(Exception ex)
        {
            return ex switch
            { 
                BadRequestExceptions e => new MessageResponse { Error = new ErrorResponse { StatusCode = 400, Message = e.Message } },
                ForbiddenExceptions e => new MessageResponse { Error = new ErrorResponse { StatusCode = 403, Message = e.Message } },
                NotFoundExceptions e => new MessageResponse { Error = new ErrorResponse { StatusCode = 404, Message = e.Message } },
                ConflictExceptions e => new MessageResponse { Error = new ErrorResponse { StatusCode = 409, Message = e.Message } },
                _ => new MessageResponse { Error = new ErrorResponse { StatusCode = 500, Message = "Unexpected error: " + ex.Message } }
            };
        }
}
