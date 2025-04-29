namespace SecureMesh.ReverseProxy.Routes.Policy;

/// <summary>
/// Template Policy of ReverseProxy Routes
/// </summary>
public static class AddAuthPolicy
{
    public static class Policies
    {
        public const string BasicPolicy = "BasicPolicy";
        public const string AdminPolicy = "AdminPolicy";
        public const string CreatorPolicy = "CreatorPolicy";
        public const string RateLimiterPolicy = "rt-sl";
    }
}

