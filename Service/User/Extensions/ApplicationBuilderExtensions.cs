namespace User.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();   
        return app;
    }
}
