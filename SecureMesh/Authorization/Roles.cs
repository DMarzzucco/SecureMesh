namespace SecureMesh.Authorization;

/// <summary>
/// Roles Levels
/// </summary>
public static class Roles
{
    public const string ADMIN = "ADMIN";
    public const string CREATOR = "CREATOR";
    public const string BASIC = "BASIC";

    public static readonly Dictionary<string, int> RolesLevels = new()
    {
        [ADMIN] = 0,
        [CREATOR] = 1,
        [BASIC] = 2
    };
}