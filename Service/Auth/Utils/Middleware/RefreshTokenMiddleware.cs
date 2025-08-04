using Microsoft.IdentityModel.Tokens;
using Auth.Cookies.Interfaces;
using Auth.JWT.Interfaces;
using Auth.Module.Services.Interfaces;

namespace Auth.Utils.Middleware
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
        public async Task InvokeAsync(HttpContext context, IJwtService tokenService, IAuthService authService, ICookieService cookieService)
        {
            var publicPaths = new[] {
                "/api/Auth/login",
                "/api/Auth/lskda_2312sd2000123sdaSD",
                "/api/Auth/registered",
                "/api/Auth/init-session",
                "/api/Auth/12349smska_wqj1n234msm949401",
                "/api/Auth/elm23019_123mskw_123fnsk",
                "/api/Auth/5413444_dsdn123fS_231_ddf",
                "/api/Auth/8382fd_1231sfw13312saeDAs12",
                "/AuthHangFireService/CountedDeleted"
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
                if (!tokenService.ValidateAuthenticationToken(accessToken))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { message = "Invalid Token" });
                    return;
                }
                if (tokenService.IsTokenExpirationSoon(accessToken))
                {
                    var payload = await authService.GetValueByCookie();

                    var refreshToken = context.Request.Cookies["RefreshToken"];
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { message = "Refresh Token is missing " });
                        return;
                    }
                    if (!tokenService.ValidateAuthenticationToken(refreshToken))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new { message = "Invalid refresh Token" });
                        return;
                    }
                    var newAccessToken = tokenService.GenerateRefreshToken(payload.SessionId, payload.User);
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