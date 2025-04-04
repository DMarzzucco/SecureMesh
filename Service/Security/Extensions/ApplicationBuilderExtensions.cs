using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using User;
using Security.Configuration.Swagger;
using Security.Utils.Filter;
using Security.Server.Service.Interfaces;
using Security.Server.Service;
using Security.Cookies.Interfaces;
using Security.Cookies;
using Security.JWT.Interfaces;
using Security.JWT;
using Security.Module.Services.Interfaces;
using Security.Module.Services;
using Security.Module.Filter;

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
            //app.UseMiddleware<>();
            app.UseCors("CorsPolicy");
            return app;
        }
    }

    /// <summary>
    /// Service builder
    /// </summary>
    public static class ServiceBuilderExtensions
    {
        public static IServiceCollection AddServiceBuilderExtensions(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();
            //JWT Configuration
            var secretKey = configuration.GetSection("JwtSettings").GetSection("seecretKey").ToString();
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException(nameof(secretKey), "Secret Key cannot be null or empty");

            service.AddAuthentication(conf =>
            {
                conf.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                conf.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(conf =>
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var signingCredential = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);

                conf.RequireHttpsMetadata = false;
                conf.SaveToken = true;
                conf.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            //Controller
            service.AddControllers(o =>
            {
                o.Filters.Add(typeof(GlobalFilterExceptions));
            }).AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.WriteIndented = true;
                o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            //scope
            service.AddScoped<GlobalFilterExceptions>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<ICookieService, CookieService>();
            service.AddScoped<IJwtService, JwtService>();
            service.AddScoped<ISecurityService, SecurityService>();
            service.AddScoped<LocalAuthFilter>();

            //Swagger Configuration
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen(o =>
            {
                o.EnableAnnotations();
                o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Security API",
                    Description = " Api of Security"
                });
                o.SchemaFilter<SwaggerSchemaFilter>();
            });
            //Cors Policy
            service.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", c =>
                {
                    c.WithOrigins("https:localhost:8888/")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            //gRPC
            service.AddGrpc();
            var httpClientHandler = new HttpClientHandler
            {
                // Just For Dev
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            service.AddGrpcClient<UserServiceGrpc.UserServiceGrpcClient>(x =>
            {
                //x.Address = new Uri("https://172.31.64.1:4080");
                x.Address = new Uri("https://localhost:4080");
                //x.Address = new Uri("https://user:4080");
                x.ChannelOptionsActions.Add(op =>
                {
                    op.HttpHandler = httpClientHandler;
                });
            });

            return service;
        }
    }
}
