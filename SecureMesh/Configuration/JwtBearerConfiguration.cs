using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SecureMesh.Configuration
{
    public static class JwtBearerConfiguration
    {
        public static IServiceCollection AddJwtBearerConfiguration(this IServiceCollection service, IConfiguration configuration)
        {
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

            service.AddAuthorizationBuilder()
                .AddPolicy("UserPolicy", pol => pol.RequireAuthenticatedUser().RequireRole("ADMIN"));

            return service;
        }
    }
}
