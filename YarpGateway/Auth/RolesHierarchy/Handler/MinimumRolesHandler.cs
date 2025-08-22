using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using YarpGateway.Auth.RolesHierarchy.RolesConst;

namespace YarpGateway.Auth.RolesHierarchy.Handler;

/// <summary>
/// Minimum Handler
/// </summary>
public class MinimumRolesHandler : AuthorizationHandler<MinimumRolesRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MinimumRolesRequirement requirement)
    {
        var userRoles = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRoles != null &&
            Roles.RolesLevels.TryGetValue(userRoles, out var userLevel) &&
            Roles.RolesLevels.TryGetValue(requirement.MinimumRole, out var requiredLevel)
           )
        {
            if (userLevel <= requiredLevel)
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}