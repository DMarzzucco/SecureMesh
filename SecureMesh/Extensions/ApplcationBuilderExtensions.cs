namespace SecureMesh.Extensions
{
    /// <summary>
    /// Application builder Extensions
    /// </summary>
    public static class ApplcationBuilderExtensions
    {
        public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthentication();

            return app;
        }
    }
    /// <summary>
    /// Service builder extensions
    /// </summary>
    public static class ServiceBuilderExtensions
    {
        public static IServiceCollection AddServiceBuilderExtensions(this IServiceCollection service, IConfiguration conf)
        {
            //Swagger Configuration
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen();
            //Yarp
            service.AddReverseProxy()
                .LoadFromConfig(conf.GetSection("ReverseProxy"));
            //controller
            service.AddControllers(o =>
            {
                //o.Filters.Add(typeof());
            });
            ///service add scope
            //service.AddScoped<>();

            //Cors Policy
            service.AddCors(x =>
            {
                x.AddPolicy("CorsPolicy", c =>
                {
                    c.WithOrigins("https://localhost:8878")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            return service;
        }
    }
}
