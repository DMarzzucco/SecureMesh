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
        // (ROLES:ADMIN)
        //Update Roles
        new RouteConfig
        {
            RouteId = "update_roles_route",
            ClusterId = "user_cluster",
            RateLimiterPolicy = Policies.BasicLimiterPolicy,
            AuthorizationPolicy = Policies.AdminPolicy,
            Match = new RouteMatch { Path = "/api/User/{id}/rm0x1" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
        },

        //(ROLES:CREATOR)
        //Get All Register
        new RouteConfig
        {
            RouteId = "all_register_route",
            ClusterId = "user_cluster",
            RateLimiterPolicy = Policies.BasicLimiterPolicy,
            AuthorizationPolicy = Policies.CreatorPolicy,
            Match = new RouteMatch { Path = "/api/User/list" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
        },
        //(ROLES:CREATOR)
        //Update User
        new RouteConfig
        {
            RouteId = "update_register_route",
            ClusterId = "user_cluster",
            RateLimiterPolicy = Policies.BasicLimiterPolicy,
            AuthorizationPolicy = Policies.CreatorPolicy,
            Match = new RouteMatch { Path = "/api/User/{id}/e90u" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
        },
        //(ROLES:CREATOR)
        //Delete Register
        new RouteConfig
        {
            RouteId = "delete_register_route",
            ClusterId = "user_cluster",
            RateLimiterPolicy = Policies.BasicLimiterPolicy,
            AuthorizationPolicy = Policies.CreatorPolicy,
            Match = new RouteMatch { Path = "/api/User/{id}/r37d" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
        },

        //Update Credentials (ROLES:BASIC)
        new RouteConfig
        {
            RouteId = "update_own_credentials",
            ClusterId = "user_cluster",
            RateLimiterPolicy = Policies.UpdateCredentials,
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/User/{id}" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
        },

        // (ROLES:BASIC)
        // User EP
        new RouteConfig
        {
            RouteId = "user_route",
            ClusterId = "user_cluster",
            RateLimiterPolicy = Policies.BasicLimiterPolicy,
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/User/{**catch-all}" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/user" } } }
        }
    ];
}
