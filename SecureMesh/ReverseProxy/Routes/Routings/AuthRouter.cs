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
        // registered
        new RouteConfig
        {
            RouteId = "register_route",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.RateLimiterPolicy,
            Match = new RouteMatch { Path = "/api/Security/registered" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        
        ///Forget Password
        new RouteConfig
        {
            RouteId = "forget_password",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.RateLimiterPolicy,
            Match = new RouteMatch { Path = "/api/Security/elm23019_123mskw_123fnsk" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },

        /// Worker (verification email)
        new RouteConfig
        {
            RouteId = "verification email",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.RateLimiterPolicy,
            Match = new RouteMatch { Path = "/api/Security/12349smska_wqj1n234msm949401" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },

        /// Worker (verification new email)
        new RouteConfig
        {
            RouteId = "verification new email",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.RateLimiterPolicy,
            Match = new RouteMatch { Path = "/api/Security/5413444_dsdn123fS_231_ddf" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        /// Worker (reset password)
        new RouteConfig
        {
            RouteId = "reset password",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.RateLimiterPolicy,
            Match = new RouteMatch { Path = "/api/Security/8382fd_1231sfw13312saeDAs12" },
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
