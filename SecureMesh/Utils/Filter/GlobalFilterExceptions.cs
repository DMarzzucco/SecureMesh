using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecureMesh.Utils.Exceptions;
using SecureMesh.Utils.Helper;

namespace SecureMesh.Utils.Filter
{
    public class GlobalFilterExceptions (ILogger<GlobalFilterExceptions> logger) : IExceptionFilter
    {
        private readonly ILogger<GlobalFilterExceptions> _logger = logger;

        public void OnException(ExceptionContext context)
        {
            this._logger.LogError(context.Exception, "Unhandled exception occurred");

            var statusCode = context.Exception switch
            {
                BadGatewayException => 502,
                _ => 500
            };

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = statusCode switch
                {
                    502 => context.Exception.Message,
                    _ => context.Exception.Message
                },
                Details = statusCode == 500 ?
                    context.Exception.InnerException?.Message : null
            };

            context.Result = new ObjectResult(response) { StatusCode = statusCode };
            context.ExceptionHandled = true;
        }
    }
}
