using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Swagger;

namespace SecureMesh.Configuration.Swagger;

/// <summary>
/// Swagger Option Configuration Proxy
/// </summary>
/// <param name="revereseProxyDocumentFilterConfigOptions"></param>
public class ConfigureSwaggerOptions(
    IOptionsMonitor<ReverseProxyDocumentFilterConfig>
     revereseProxyDocumentFilterConfigOptions) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly ReverseProxyDocumentFilterConfig _reverseProxyDocumentFilterConfigOptions = revereseProxyDocumentFilterConfigOptions.CurrentValue;
    public void Configure(SwaggerGenOptions options)
    {
        var filterDescriptor = new List<FilterDescriptor>();

        foreach (var cluster in _reverseProxyDocumentFilterConfigOptions.Clusters)
        {
            options.SwaggerDoc(cluster.Key, new OpenApiInfo { Title = cluster.Key, Version = cluster.Key });
        }

        filterDescriptor.Add(new FilterDescriptor
        {
            Type = typeof(ReverseProxyDocumentFilter),
            Arguments = Array.Empty<object>()
        });

        options.DocumentFilterDescriptors = filterDescriptor;
    }
}
