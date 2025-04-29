using Yarp.ReverseProxy.Configuration;
using static SecureMesh.ReverseProxy.Routes.Policy.AddAuthPolicy;

namespace SecureMesh.ReverseProxy.Routes.Routings;

/// <summary>
/// API Auth Routings 
/// </summary>
public static class AuthRouter
{
    public static IReadOnlyList<RouteConfig> GetRoutes() =>
    [
        //Login 
        new RouteConfig
        {
            RouteId = "login_route",
            ClusterId = "auth_cluster",
            Match = new RouteMatch { Path = "/api/Security/login" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        //Security EP
        new RouteConfig
        {
            RouteId = "auth_route",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/Security/{**catch-all}" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },

    ];
}
