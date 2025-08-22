using IdentifyService.Module.Stub;
using IdentifyService.Utils.Middleware;

namespace IdentifyService.Extensions
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
            app.UseEndpoints(e => { e.MapGrpcService<RemoveIdentityProviderRelation>();  });
            app.UseCors("CorsPolicy");
            return app;
        }
    }
}
