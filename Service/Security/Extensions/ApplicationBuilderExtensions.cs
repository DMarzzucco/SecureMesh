using Security.Utils.Middleware;

namespace Security.Extensions
{
    /// <summary>
    /// Application Builder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMiddleware<RefreshTokenMiddleware>();
            app.UseCors("CorsPolicy");
            return app;
        }
    }
}
