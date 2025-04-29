using SecureMesh.ReverseProxy.Routes.Routings;
using Yarp.ReverseProxy.Configuration;

namespace SecureMesh.ReverseProxy.Routes;

public static class RoutesDefinitions
{
    public static IReadOnlyList<RouteConfig> GetRoutes() =>
        [
            .. AuthRouter.GetRoutes(),
            .. UserRouter.GetRoutes()
        ];

    // AuthRouter.GetRoutes()
    // .Concat(UserRouter.GetRoutes())
    // .ToList();
}
