using Yarp.ReverseProxy.Configuration;

namespace SecureMesh.ReverseProxy.Routes;

public static class RoutesDefinitions
{
    public static IReadOnlyList<RouteConfig> GetRoutes()
    {
        return new[]
        {
            new RouteConfig
            {
                RouteId = "login_route",
                ClusterId = "auth_cluster",
                Match = new RouteMatch { Path = "/api/Security/login" },
                Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
            },
            new RouteConfig
            {
                RouteId = "auth_route",
                ClusterId = "auth_cluster",
                AuthorizationPolicy = "AdminPolicy",
                Match = new RouteMatch { Path = "/api/Security/{**catch-all}" },
                Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
            },
            new RouteConfig
            {
                RouteId = "user_route",
                ClusterId = "user_cluster",
                RateLimiterPolicy = "rt-sl",
                AuthorizationPolicy = "BasicPolicy",
                Match = new RouteMatch { Path = "/api/User/{**catch-all}" },
                Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
            },
            new RouteConfig
            {
                RouteId = "register_route",
                ClusterId = "user_cluster",
                RateLimiterPolicy = "rt-sl",
                Match = new RouteMatch { Path = "/api/User/register" },
                Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
            }
        };
    }

}