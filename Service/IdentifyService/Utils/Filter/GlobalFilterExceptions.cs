using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using IdentifyService.Utils.Exceptions;

namespace IdentifyService.Utils.Filter
{
    public class GlobalFilterExceptions(ILogger<GlobalFilterExceptions> logger) : IExceptionFilter
    {
        private readonly ILogger<GlobalFilterExceptions> _logger = logger;

        public void OnException(ExceptionContext context)
        {
            var statusCode = context.Exception switch
            {
                BadRequestExceptions => 400,
                UnauthorizedAccessException => 401,
                ForbiddenExceptions => 403,
                SecurityTokenExpiredException => 403,
                SecurityTokenSignatureKeyNotFoundException => 403,
                NotFoundExceptions => 404,

                RpcException grpcEx => grpcEx.StatusCode switch
                {
                    StatusCode.NotFound => 404,
                    StatusCode.Unauthenticated => 401,
                    StatusCode.InvalidArgument => 400,
                    _ => 500
                },
                ConflictExceptions => 409,
                TooManyRequestsException => 429,
                _ => 500
            };
            var message = context.Exception switch
            {
                BadRequestExceptions ex => ex.Message,
                UnauthorizedAccessException ex => ex.Message,
                ForbiddenExceptions ex => ex.Message,
                SecurityTokenExpiredException => "Token has expired",
                SecurityTokenSignatureKeyNotFoundException => "Token is invalid",
                RpcException grpcEx => grpcEx.StatusCode switch
                {
                    StatusCode.NotFound => grpcEx.Status.Detail,
                    StatusCode.Unauthenticated => grpcEx.Status.Detail,
                    StatusCode.InvalidArgument => grpcEx.Status.Detail,
                    _ => grpcEx.Status.Detail
                },
                KeyNotFoundException ex => ex.Message,
                ConflictExceptions ex => ex.Message,
                TooManyRequestsException ex => ex.Message,
                _ => context.Exception.Message
            };

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Details = statusCode == 500 ?
                    context.Exception.InnerException?.Message : null
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }

        public class ErrorResponse
        {
            public int StatusCode { get; set; }
            public required string Message { get; set; }
            public string? Details { get; set; }
        }
    }
}
