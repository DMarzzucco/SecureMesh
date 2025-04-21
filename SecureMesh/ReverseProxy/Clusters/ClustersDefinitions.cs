using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SecureMesh.ReverseProxy.Routes;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Swagger;

namespace SecureMesh.ReverseProxy.Clusters;

public static class ClustersDefinitions
{
    public static IReadOnlyList<ClusterConfig> GetCluster()
    {
        return new[]
        {
            new ClusterConfig
            {
                ClusterId = "user_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "user", new DestinationConfig { Address = "https://localhost:4080" } }
                }
            },
            new ClusterConfig
            {
                ClusterId = "auth_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "auth", new DestinationConfig { Address = "https://localhost:5090" } }
                }
            }
        };
    }
}

/// <summary>
/// Swagger Document Proxy
/// </summary>
public static class SwaggerDocumentProxy
{
    public static ReverseProxyDocumentFilterConfig GetSwaggerConfig()
    {
        return new ReverseProxyDocumentFilterConfig
        {
            Routes = RoutesDefinitions.GetRoutes().ToDictionary(_ => _.RouteId, _ => _),
            Clusters = new Dictionary<string, ReverseProxyDocumentFilterConfig.Cluster>
            {
                {
                    "user_cluster", new ReverseProxyDocumentFilterConfig.Cluster
                    {
                        Destinations = new Dictionary<string, ReverseProxyDocumentFilterConfig.Cluster.Destination>
                        {
                            {
                                "user", new ReverseProxyDocumentFilterConfig.Cluster.Destination
                                {
                                    Address = "https://localhost:4080",
                                    Swaggers = new[]
                                    {
                                        new ReverseProxyDocumentFilterConfig.Cluster.Destination.Swagger
                                        {
                                            PrefixPath = "/proxy-user",
                                            Paths = new[] {"/swagger/v1/swagger.json"}
                                        }
                                    },
                                    AccessTokenClientName = null
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
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