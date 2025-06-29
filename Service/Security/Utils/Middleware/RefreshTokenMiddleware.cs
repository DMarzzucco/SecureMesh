﻿using Microsoft.IdentityModel.Tokens;
using Security.Cookies.Interfaces;
using Security.JWT.Interfaces;
using Security.Module.Services.Interfaces;

namespace Security.Utils.Middleware
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public RefreshTokenMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        /// <summary>
        /// Invoke Middleware
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tokenService"></param>
        /// <param name="authService"></param>
        /// <param name="cookieService"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, IJwtService tokenService, ISecurityService authService, ICookieService cookieService)
        {
            var publicPaths = new[] {
                "/api/Security/login",
                "/api/Security/registered",
                "/api/Security/12349smska_wqj1n234msm949401",
                "/api/Security/elm23019_123mskw_123fnsk",
                "/api/Security/5413444_dsdn123fS_231_ddf",
                "/api/Security/8382fd_1231sfw13312saeDAs12"
                  };

            var path = context.Request.Path.Value;
            if (publicPaths.Contains(path))
            {
                await _next(context);
                return;
            }

            var accessToken = context.Request.Cookies["Authentication"];

            if (string.IsNullOrEmpty(accessToken))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Token missing" });
                return;
            }
            try
            {
                if (!tokenService.ValidateToken(accessToken))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { message = "Invalid Token" });
                    return;
                }
                if (tokenService.IsTokenExpirationSoon(accessToken))
                {
                    var user = await authService.GetUserByCookie();

                    var refreshToken = context.Request.Cookies["RefreshToken"];
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { message = "Refresh Token is missing " });
                        return;
                    }
                    if (!tokenService.ValidateToken(refreshToken))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new { message = "Invalid refresh Token" });
                        return;
                    }
                    var newAccessToken = tokenService.RefreshToken(user);
                    cookieService.SetTokenCookies(context.Response, newAccessToken);
                }
            }
            catch (SecurityTokenExpiredException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                return;
            }

            await this._next(context);
        }
    }
}