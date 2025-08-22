using Microsoft.AspNetCore.Mvc.Filters;
using IdentifyService.Module.DTOs;
using IdentifyService.Module.Services.Interfaces;

namespace IdentifyService.Module.Filter
{
    public class LocalAuthFilter : IAsyncActionFilter
    {
        private readonly IIdentityProviderService _service;

        public LocalAuthFilter(IIdentityProviderService service)
        {
            _service = service;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("body", out var bodyObj) && bodyObj is LoginDTO body)
            {
                var user = await this._service.ValidateUserCredential(body);
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
