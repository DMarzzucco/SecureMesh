using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecureMesh.Utils.Exceptions;

namespace SecureMesh.Utils.Filter
{
    public class GlobalFilterExceptions : IExceptionFilter
    {
        private readonly ILogger<GlobalFilterExceptions> _logger;

        public GlobalFilterExceptions(ILogger<GlobalFilterExceptions> logger) => _logger = logger;

        public void OnException(ExceptionContext context)
        {
            this._logger.LogError(context.Exception, "Unhandled exception occurred");

            var statusCode = context.Exception switch
            {
                ArgumentNullException => 400,
                UnauthorizedAccessException => 401,
                ForbiddenException => 403,
                BadGatewayException => 502,
                _ => 500
            };

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = statusCode switch
                {
                    400 => context.Exception.Message,
                    401 => context.Exception.Message,
                    403 => context.Exception.Message,
                    502 => context.Exception.Message,
                    _ => context.Exception.Message
                },
                Details = statusCode == 500 ?
                    context.Exception.InnerException?.Message : null
            };

            context.Result = new ObjectResult(response) { StatusCode = statusCode };
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
