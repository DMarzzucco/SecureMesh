using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SecureMesh.Utils.Helper;

namespace SecureMesh.Utils.Auth;

public static class JwtEventHandlers
{
    public static JwtBearerEvents GetJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            //Cookie Authentication
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["Authentication"];
                
                if (!string.IsNullOrEmpty(accessToken))
                    context.Token = accessToken;
                
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                        
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Unauthorized",
                    Details = null
                });
                await context.Response.WriteAsync(json);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(new ErrorResponse
                {
                    StatusCode = 403,
                    Message = "Forbidden: You do not have permission to access this resource",
                    Details = null
                });
                await context.Response.WriteAsync(json);
            }
        };
    }
}