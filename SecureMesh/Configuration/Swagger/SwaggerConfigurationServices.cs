using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SecureMesh.Configuration.Swagger;

public static class SwaggerConfigurationServices
{
    public static IServiceCollection AddSwaggerConfigurationService(this IServiceCollection service)
    {

        service.AddHttpClient("SwaggerCertByPass")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

        service.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen(c =>
        {
            c.DocumentFilter<ReverseProxyDocumentFilterSwagger>();
        });

        return service;
    }
}
