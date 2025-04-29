using Yarp.ReverseProxy.Configuration;
using static SecureMesh.ReverseProxy.Routes.Policy.AddAuthPolicy;

namespace SecureMesh.ReverseProxy.Routes.Routings;

/// <summary>
/// API User Routings 
/// </summary>
public static class UserRouter
{
    public static IReadOnlyList<RouteConfig> GetRoutes() =>
    [
        //Register 
        new RouteConfig
        {
            RouteId = "register_route",
            ClusterId = "user_cluster",
            RateLimiterPolicy = Policies.RateLimiterPolicy,
            Match = new RouteMatch { Path = "/api/User/register" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
        },
        // User EP
        new RouteConfig
        {
            RouteId = "user_route",
            ClusterId = "user_cluster",
            RateLimiterPolicy = Policies.RateLimiterPolicy,
            AuthorizationPolicy = Policies.AdminPolicy,
            Match = new RouteMatch { Path = "/api/User/{**catch-all}" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
        }
    ];
}
