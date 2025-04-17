using Yarp.ReverseProxy.Configuration;

namespace SecureMesh.ReverseProxy.Routes;

public static class RoutesDefinitions
{
    public static IReadOnlyList<RouteConfig> GetRoutes()
    {
        return new[]
        {
            //Login 
            new RouteConfig
            {
                RouteId = "login_route",
                ClusterId = "auth_cluster",
                Match = new RouteMatch { Path = "/api/Security/login" },
                Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
            },
            // Register
            new RouteConfig
            {
                RouteId = "register_route",
                ClusterId = "user_cluster",
                RateLimiterPolicy = "rt-sl",
                Match = new RouteMatch { Path = "/api/User/register" },
                Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
            },
            //Security EP
            new RouteConfig
            {
                RouteId = "auth_route",
                ClusterId = "auth_cluster",
                AuthorizationPolicy = "BasicPolicy",
                Match = new RouteMatch { Path = "/api/Security/{**catch-all}" },
                Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
            },
            // User EP
            new RouteConfig
            {
                RouteId = "user_route",
                ClusterId = "user_cluster",
                RateLimiterPolicy = "rt-sl",
                AuthorizationPolicy = "AdminPolicy",
                Match = new RouteMatch { Path = "/api/User/{**catch-all}" },
                Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
            }
        };
    }
}