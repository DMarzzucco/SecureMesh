using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace SecureMesh.ReverseProxy
{
    public static class ReverseProxyConfig
    {
        public static IServiceCollection AddReverseProxyConfig(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddReverseProxy()
                 .LoadFromMemory(
                    GetRoutes().ToList(), GetCluster().ToList()
                )
                 .AddTransforms(tr =>
                 {
                     tr.AddRequestTransform(async ctx =>
                     {
                         if (ctx.HttpContext.User.Identity.IsAuthenticated)
                         {
                             var token = await ctx.HttpContext.GetTokenAsync("Authentication");

                             if (string.IsNullOrEmpty(token))
                                 throw new UnauthorizedAccessException("token is missing in api gateway");

                             ctx.ProxyRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                         }
                     });
                 });
            return service;
        }

        public static IEnumerable<RouteConfig> GetRoutes()
        {
            return new[] {
                new RouteConfig
                {
                    RouteId = "auth_route",
                    ClusterId = "auth_cluster",
                    Match = new RouteMatch { Path = "/auth/{**catch-all}"},
                    Transforms = new []{new Dictionary<string, string>{ { "PathRemovePrefix", "/auth"} }}
                },
                new RouteConfig
                {
                    RouteId = "user_route",
                    ClusterId = "user_cluster",
                    Match = new RouteMatch { Path = "/user/{**catch-all}"},
                    Transforms = new []{new Dictionary<string, string>{ { "PathRemovePrefix", "/user"} }}

                }
            };
        }

        public static IEnumerable<ClusterConfig> GetCluster()
        {
            return new[] {
                new ClusterConfig {
                    ClusterId = "user_cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {"user", new DestinationConfig { Address = "https://localhost:4080"} }
                    }
                },
                new ClusterConfig {
                    ClusterId = "auth_cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        { "auth", new DestinationConfig { Address = "https://localhost:5090"} }
                    }
                }
            };
        }
    }
}
