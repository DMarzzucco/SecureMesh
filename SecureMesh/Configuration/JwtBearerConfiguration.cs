using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SecureMesh.Authorization;
using SecureMesh.Utils.Auth;

namespace SecureMesh.Configuration
{
    public static class JwtBearerConfiguration
    {
        public static IServiceCollection AddJwtBearerConfiguration(this IServiceCollection service,
            IConfiguration configuration)
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
                conf.Events = JwtEventHandlers.GetJwtBearerEvents();
            });

            service.AddAuthorization(op =>
            {
                op.AddPolicy("AdminPolicy", pol
                    => pol.RequireAuthenticatedUser()
                        .Requirements.Add(new MinimumRolesRequirement
                            (Roles.ADMIN)));

                op.AddPolicy("CreatorPolicy", pol
                    => pol.RequireAuthenticatedUser().Requirements.Add(new MinimumRolesRequirement
                        (Roles.CREATOR)));
                
                op.AddPolicy("BasicPolicy", pol
                    => pol.RequireAuthenticatedUser().Requirements.Add(new MinimumRolesRequirement
                        (Roles.BASIC)));
            });

            return service;
        }
    }
}