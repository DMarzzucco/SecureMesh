using Microsoft.AspNetCore.Authorization;

namespace YarpGateway.Auth.RolesHierarchy;

/// <summary>
/// Minimum Roles
/// </summary>
public class MinimumRolesRequirement(string minimumRole) : IAuthorizationRequirement
{
    public string MinimumRole { get; } = minimumRole;
}