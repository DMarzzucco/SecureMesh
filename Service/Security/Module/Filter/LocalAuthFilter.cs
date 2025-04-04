using Microsoft.AspNetCore.Mvc.Filters;
using Security.Module.DTOs;
using Security.Module.Services.Interfaces;

namespace Security.Module.Filter
{
    public class LocalAuthFilter : IAsyncActionFilter
    {
        private readonly ISecurityService _service;

        public LocalAuthFilter(ISecurityService service)
        {
            _service = service;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("body", out var bodyObj) && bodyObj is LoginDTO body)
            {
                var user = await this._service.ValidateUser(body);
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
