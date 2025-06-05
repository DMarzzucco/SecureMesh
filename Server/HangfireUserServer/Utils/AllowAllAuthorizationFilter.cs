using System;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace HangfireUserServer.Utils;

public class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
