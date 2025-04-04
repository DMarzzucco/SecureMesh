using User.Module.Stubs;

namespace User.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(e => {
            e.MapGrpcService<UserServiceGrpcImpl>();
        });
        app.UseStaticFiles();
        app.UseCors("CorsPolicy");

        return app;
    }
}
