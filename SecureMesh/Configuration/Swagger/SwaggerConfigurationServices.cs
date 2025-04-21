using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using SecureMesh.ReverseProxy.Clusters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SecureMesh.Extensions;

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

/// <summary>
/// Client factory
/// </summary>
public class ReverseProxyDocumentFilterSwagger (IHttpClientFactory httpClientFactory) : IDocumentFilter
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    /// <summary>
    /// Apply Documentation
    /// </summary>
    /// <param name="swaggerDoc"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var client = this._httpClientFactory.CreateClient("SwaggerCertByPass");

        var swaggerJson = client.GetStringAsync("https://localhost:4080/swagger/v1/swagger.json").Result;
        
        // Leer el JSON como OpenApiDocument
        var reader = new OpenApiStringReader();
        var remoteDoc = reader.Read(swaggerJson, out var diagnostic);

        // Combinar los paths (rutas) del swagger remoto con los locales
        foreach (var path in remoteDoc.Paths)
        {
            swaggerDoc.Paths[path.Key] = path.Value;
        }

        // (Opcional) Combinar los schemas
        foreach (var schema in remoteDoc.Components.Schemas)
        {
            if (!swaggerDoc.Components.Schemas.ContainsKey(schema.Key))
            {
                swaggerDoc.Components.Schemas[schema.Key] = schema.Value;
            }
        }
    }
}