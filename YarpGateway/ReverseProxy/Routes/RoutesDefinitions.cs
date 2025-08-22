using YarpGateway.ReverseProxy.Routes.Routings;
using Yarp.ReverseProxy.Configuration;

namespace YarpGateway.ReverseProxy.Routes;

public static class RoutesDefinitions
{
    public static IReadOnlyList<RouteConfig> GetRoutes() =>
        [
            .. IdpRouter.GetRoutes(),
            .. UmsRouter.GetRoutes()
        ];
}