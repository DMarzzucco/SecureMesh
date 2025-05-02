using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using User.Utils.Exceptions;

namespace User.Utils.Filters;

public class GlobalFilterExceptions:IExceptionFilter
{
    private readonly ILogger<GlobalFilterExceptions> _logger;

    public GlobalFilterExceptions(ILogger<GlobalFilterExceptions> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        this._logger.LogError(context.Exception, "Unhandled exception occurred");

        var statusCode = context.Exception switch
        {
            BadRequestExceptions => 400,
            ForbiddenExceptions => 403,
            NotFoundExceptions => 404,
            ConflictExceptions => 409,
            _ => 500
        };
        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = statusCode switch
            {
                400 => context.Exception.Message,
                403 => context.Exception.Message,
                404 => context.Exception.Message,
                409 => context.Exception.Message,
                _ => context.Exception.Message
            },
            Detail = statusCode == 500 ?
                context.Exception.InnerException?.Message : null
        };
        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode,
        };
        context.ExceptionHandled = true;
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public required string Message { get; set; }
        public string? Detail { get; set; }
    }
}