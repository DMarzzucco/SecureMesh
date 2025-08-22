using Yarp.ReverseProxy.Configuration;
using static YarpGateway.Auth.RateLimiterPolicy.Const.AddAuthPolicy;

namespace YarpGateway.ReverseProxy.Routes.Routings;

/// <summary>
/// API User Routings 
/// </summary>
public static class UmsRouter
{
    public static IReadOnlyList<RouteConfig> GetRoutes() =>
    [
        // (ROLES:ADMIN)
        //Update Roles
        new RouteConfig
        {
            RouteId = "update_roles_route",
            ClusterId = "ums_cluster",
            AuthorizationPolicy = Policies.AdminPolicy,
            Match = new RouteMatch { Path = "/api/UMS/{id}/u9d473-r0l35" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/user" } }]
        },

        //(ROLES:CREATOR)
        //Get All Register
        new RouteConfig
        {
            RouteId = "all_register_route",
            ClusterId = "ums_cluster",
            AuthorizationPolicy = Policies.CreatorPolicy,
            Match = new RouteMatch { Path = "/api/UMS" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/user" } }]
        },
        //(ROLES:CREATOR)
        //Delete Register
        new RouteConfig
        {
            RouteId = "delete_register_route",
            ClusterId = "ums_cluster",
            AuthorizationPolicy = Policies.CreatorPolicy,
            Match = new RouteMatch { Path = "/api/UMS/{id}/r3m0v3-4nn-4cc0yn7" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/user" } }]
        },

        //Update Credentials (ROLES:BASIC)
        new RouteConfig
        {
            RouteId = "update_own_credentials",
            ClusterId = "ums_cluster",
            RateLimiterPolicy = Policies.UpdateCredentials,
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/UMS/{id}/edit-registered" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/user" } }]
        },
        ///Forget Password
        new RouteConfig
        {
            RouteId = "forget_password",
            ClusterId = "idp_cluster",
            RateLimiterPolicy = Policies.ForgetPasswordPolicy,
            Match = new RouteMatch { Path = "/api/UMS/elm23019_123mskw_123fnsk" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/UMS" } }]
        },
        /// Worker (verification new email)
        new RouteConfig
        {
            RouteId = "verification new email",
            ClusterId = "idp_cluster",
            RateLimiterPolicy = Policies.VerifyNewEmailPolicy,
            Match = new RouteMatch { Path = "/api/UMS/5413444_dsdn123fS_231_ddf" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/UMS" } }]
        },
         
        /// Worker (reset password)
        new RouteConfig
        {
            RouteId = "reset password",
            ClusterId = "idp_cluster",
            RateLimiterPolicy = Policies.ResetPasswordPolicy,
            Match = new RouteMatch { Path = "/api/UMS/8382fd_1231sfw13312saeDAs12" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/UMS" } }]
        },

        // (ROLES:BASIC)
        // User EP
        new RouteConfig
        {
            RouteId = "user_route",
            ClusterId = "ums_cluster",
            // RateLimiterPolicy = Policies.BasicLimiterPolicy,
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/UMS/{**catch-all}" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/user" } }]
        }
    ];
}
