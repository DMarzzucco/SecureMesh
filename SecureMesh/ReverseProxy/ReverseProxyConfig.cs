using Microsoft.AspNetCore.RateLimiting;
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
                    GetRoutes(), GetCluster()
                )
                 .AddTransforms(tr =>
                 {
                     tr.AddRequestTransform(async ctx =>
                     {
                         var authHeader = ctx.HttpContext.Request.Headers["Authorization"].ToString();

                         if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                         {
                             var token = authHeader.Substring("Bearer ".Length).Trim();
                             ctx.ProxyRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                         }
                     });
                 });
            /// Rate Limit Config
            service.AddRateLimiter(op =>
            {
                op.AddFixedWindowLimiter("rt-sl", op =>
                {
                    op.PermitLimit = 3;
                    op.Window = TimeSpan.FromSeconds(3);
                    //op.QueueLimit = 1;
                    //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            return service;
        }

        public static IReadOnlyList<RouteConfig> GetRoutes()
        {
            return new[] {
                    new RouteConfig
                    {
                        RouteId = "auth_route",
                        ClusterId = "auth_cluster",
                        Match = new RouteMatch { Path = "/api/Security/{**catch-all}"},
                        Transforms = new []{new Dictionary<string, string>{ { "PathRemovePrefix", "/auth"} }}
                    },
                    new RouteConfig
                    {
                        RouteId = "user_route",
                        ClusterId = "user_cluster",
                        RateLimiterPolicy = "rt-sl",
                        AuthorizationPolicy = "UserPolicy",
                        Match = new RouteMatch { Path = "/api/User/{**catch-all}"},
                        Transforms = new []{new Dictionary<string, string>{ { "PathRemovePrefix", "/user"} }}

                    }
                };
        }

        public static IReadOnlyList<ClusterConfig> GetCluster()
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
