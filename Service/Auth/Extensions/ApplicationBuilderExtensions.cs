using Auth.Module.Stub.Hangfire;
using Auth.Utils.Middleware;
using AuthHangFire.Proto;

namespace Auth.Extensions
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
            app.UseRouting();
            app.UseEndpoints(e => { e.MapGrpcService<AuthHangfireServiceGrpc>();  });
            app.UseCors("CorsPolicy");
            return app;
        }
    }
}
