using Microsoft.AspNetCore.Authorization;

namespace SecureMesh.Authorization;

/// <summary>
/// Minimum Roles
/// </summary>
public class MinimumRolesRequirement : IAuthorizationRequirement
{
    public string MinimumRole { get; }

    public MinimumRolesRequirement(string minimumRole)
    {
        MinimumRole = minimumRole;
    }
}