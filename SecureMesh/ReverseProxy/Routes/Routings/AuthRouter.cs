using Yarp.ReverseProxy.Configuration;
using static SecureMesh.ReverseProxy.Policy.AddAuthPolicy;

namespace SecureMesh.ReverseProxy.Routes.Routings;

/// <summary>
/// API Auth Routings 
/// </summary>
public static class AuthRouter
{
    public static IReadOnlyList<RouteConfig> GetRoutes() =>
    [
        // registered
        new RouteConfig
        {
            RouteId = "register_route",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.RegisterPolicy,
            Match = new RouteMatch { Path = "/api/Auth/registered" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        //Login 
        new RouteConfig
        {
            RouteId = "login_route",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.LoginPolicy,
            Match = new RouteMatch { Path = "/api/Auth/login" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        // start session
        new RouteConfig
        {
            RouteId = "init_sessions_route",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.InitSessionPolicy,
            Match = new RouteMatch { Path = "/api/Auth/init-session" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        // session list
        new RouteConfig
        {
            RouteId = "list_session_route",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/Auth/sessions-list" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        // Delete Session by Id
        new RouteConfig
        {
            RouteId = "delete_session_route",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.RemoveSessionPolicy,
            Match = new RouteMatch { Path = "/api/Auth/session-delete" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        // logout
        new RouteConfig
        {
            RouteId = "logout_route",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.LogOutPolicy,
            Match = new RouteMatch { Path = "/api/Auth/logout" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        // remove user
        new RouteConfig
        {
            RouteId = "remove_user_route",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.RemoveUserPolicy,
            Match = new RouteMatch { Path = "/api/Auth/remove_ownaccount" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        ///Forget Password
        new RouteConfig
        {
            RouteId = "forget_password",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.ForgetPasswordPolicy,
            Match = new RouteMatch { Path = "/api/Auth/elm23019_123mskw_123fnsk" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        // Generate 2FA Code
        new RouteConfig
        {
            RouteId = "2fa_code_generate_route",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.Generate2FACodePolicy,
            Match = new RouteMatch { Path = "/api/Auth/2faC@d363n3r4t3" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        ///Update Email
        new RouteConfig
        {
            RouteId = "update-email",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.UpdateEmailPolicy,
            Match = new RouteMatch { Path = "/api/Auth/r3f1orm@2-ema1l@213" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        ///Update password
        new RouteConfig
        {
            RouteId = "update-password",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            RateLimiterPolicy = Policies.PasswordUpdateLimit,
            Match = new RouteMatch { Path = "/api/Auth/upd4t3-p455w@rd" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        /// Worker (RBA Verification)
        new RouteConfig
        {
            RouteId = "init_session_route",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.VerifyRBAPolicy,
            Match = new RouteMatch { Path = "/api/Auth/lskda_2312sd2000123sdaSD" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        /// Worker (verification email)
        new RouteConfig
        {
            RouteId = "verification email",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.VerifyEmailPolicy,
            Match = new RouteMatch { Path = "/api/Auth/12349smska_wqj1n234msm949401" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        /// Worker (verification new email)
        new RouteConfig
        {
            RouteId = "verification new email",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.VerifyNewEmailPolicy,
            Match = new RouteMatch { Path = "/api/Auth/5413444_dsdn123fS_231_ddf" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        /// Worker (reset password)
        new RouteConfig
        {
            RouteId = "reset password",
            ClusterId = "auth_cluster",
            RateLimiterPolicy = Policies.ResetPasswordPolicy,
            Match = new RouteMatch { Path = "/api/Auth/8382fd_1231sfw13312saeDAs12" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },
        //Auth EP
        new RouteConfig
        {
            RouteId = "auth_route",
            ClusterId = "auth_cluster",
            AuthorizationPolicy = Policies.BasicPolicy,
            Match = new RouteMatch { Path = "/api/Auth/{**catch-all}" },
            Transforms = new[] { new Dictionary<string, string> { { "PathRemovePrefix", "/auth" } } }
        },

    ];
}
