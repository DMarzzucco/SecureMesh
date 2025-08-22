using System;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace AccountDeletionScheduler.Utils;

public class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
