using User.Utils.Exceptions;
namespace User.Module.Stubs.Handlers
{
    public class HandlerGrpcExceptions
    {
        public ValidationResponse InvokeExceptions(Exception ex)
        {
            return ex switch
            {
                BadRequestExceptions e => new ValidationResponse { Error = new ErrorResponse { StatusCode = 400, Message = e.Message } },
                ForbiddenExceptions e => new ValidationResponse { Error = new ErrorResponse { StatusCode = 403, Message = e.Message } },
                NotFoundExceptions e => new ValidationResponse { Error = new ErrorResponse { StatusCode = 404, Message = e.Message } },
                ConflictExceptions e => new ValidationResponse { Error = new ErrorResponse { StatusCode = 409, Message = e.Message } },
                _ => new ValidationResponse { Error = new ErrorResponse { StatusCode = 500, Message = "Unexpected error: " + ex.Message } }
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
}
