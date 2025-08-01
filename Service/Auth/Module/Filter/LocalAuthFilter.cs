using Microsoft.AspNetCore.Mvc.Filters;
using Auth.Module.DTOs;
using Auth.Module.Services.Interfaces;

namespace Auth.Module.Filter
{
    public class LocalAuthFilter : IAsyncActionFilter
    {
        private readonly IAuthService _service;

        public LocalAuthFilter(IAuthService service)
        {
            _service = service;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("body", out var bodyObj) && bodyObj is LoginDTO body)
            {
                var user = await this._service.ValidateUserCredentials(body);
                context.HttpContext.Items["User"] = user;
            }
            else
            {
                context.HttpContext.Items["User"] = null;
            }
            await next();
        }
    }
}
