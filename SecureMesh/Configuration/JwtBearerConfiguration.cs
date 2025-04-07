using Microsoft.IdentityModel.Tokens;

namespace SecureMesh.Configuration
{
    public static class JwtBearerConfiguration
    {
        public static IServiceCollection AddJwtBearerConfiguration(this IServiceCollection service)
        {
            service.AddAuthentication("Bearer").AddJwtBearer("Bearer", op =>
            {
                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

            service.AddAuthorization(op => { });

            return service;
        }
    }
}
