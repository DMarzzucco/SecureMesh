namespace SecureMesh.Extensions
{
    /// <summary>
    /// Application builder Extensions
    /// </summary>
    public static class ApplcationBuilderExtensions
    {
        public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();

            return app;
        }
    }
}