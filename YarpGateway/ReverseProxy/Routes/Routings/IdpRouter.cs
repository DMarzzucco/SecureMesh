using Yarp.ReverseProxy.Configuration;
using static YarpGateway.Auth.RateLimiterPolicy.Const.AddAuthPolicy;

namespace YarpGateway.ReverseProxy.Routes.Routings;

/// <summary>
/// API Auth Routings 
/// </summary>
public static class IdpRouter
{
    public static IReadOnlyList<RouteConfig> GetRoutes() =>
    [
        // registered
        new RouteConfig
        {
            RouteId = "register_route",
            ClusterId = "idp_cluster",
            RateLimiterPolicy = Policies.RegisterPolicy,
            Match = new RouteMatch { Path = "/api/Idp/registered" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        //Login 
        new RouteConfig
        {
            RouteId = "login_route",
            ClusterId = "idp_cluster",
            RateLimiterPolicy = Policies.LoginPolicy,
            Match = new RouteMatch { Path = "/api/Idp/login" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        // start session
        new RouteConfig
        {
            RouteId = "init_sessions_route",
            ClusterId = "idp_cluster",
            RateLimiterPolicy = Policies.InitSessionPolicy,
            Match = new RouteMatch { Path = "/api/Idp/init-session" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        // session list
        new RouteConfig
        {
            RouteId = "list_session_route",
            ClusterId = "idp_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/Idp/sessions-list" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        // Delete Session by Id
        new RouteConfig
        {
            RouteId = "delete_session_route",
            ClusterId = "idp_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.RemoveSessionPolicy,
            Match = new RouteMatch { Path = "/api/Idp/session-delete" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        // logout
        new RouteConfig
        {
            RouteId = "logout_route",
            ClusterId = "idp_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.LogOutPolicy,
            Match = new RouteMatch { Path = "/api/Idp/logout" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        // remove own account user
        new RouteConfig
        {
            RouteId = "remove_user_route",
            ClusterId = "idp_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.RemoveUserPolicy,
            Match = new RouteMatch { Path = "/api/Idp/remove_ownaccount" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },

        // Generate 2FA Code
        new RouteConfig
        {
            RouteId = "2fa_code_generate_route",
            ClusterId = "idp_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.Generate2FACodePolicy,
            Match = new RouteMatch { Path = "/api/Idp/2faC@d363n3r4t3" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        ///Update Email
        new RouteConfig
        {
            RouteId = "update-email",
            ClusterId = "idp_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.UpdateEmailPolicy,
            Match = new RouteMatch { Path = "/api/Idp/r3f1orm@2-ema1l@213" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        ///Update password
        new RouteConfig
        {
            RouteId = "update-password",
            ClusterId = "idp_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.PasswordUpdateLimit,
            Match = new RouteMatch { Path = "/api/Idp/upd4t3-p455w@rd" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        /// Worker (RBA Verification)
        new RouteConfig
        {
            RouteId = "init_session_route",
            ClusterId = "idp_cluster",
            RateLimiterPolicy = Policies.VerifyRBAPolicy,
            Match = new RouteMatch { Path = "/api/Idp/lskda_2312sd2000123sdaSD" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        /// Worker (verification email)
        new RouteConfig
        {
            RouteId = "verification email",
            ClusterId = "idp_cluster",
            RateLimiterPolicy = Policies.VerifyEmailPolicy,
            Match = new RouteMatch { Path = "/api/Idp/12349smska_wqj1n234msm949401" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },
        //Idp EP
        new RouteConfig
        {
            RouteId = "auth_route",
            ClusterId = "idp_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/Idp/{**catch-all}" },
            Transforms = [new Dictionary<string, string> { { "PathRemovePrefix", "/Idp" } }]
        },

    ];
}
