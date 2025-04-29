using SecureMesh.ReverseProxy.Routes;
using Yarp.ReverseProxy.Swagger;

namespace SecureMesh.Configuration.Swagger;

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