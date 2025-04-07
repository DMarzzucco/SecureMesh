namespace SecureMesh.Utils.Exceptions
{
    /// <summary>
    /// 403
    /// </summary>
    public class ForbiddenException(string message) : Exception(message)
    {
    }
    /// <summary>
    /// 502
    /// </summary>
    public class BadGatewayException(string message) : Exception(message)
    {
    }
}
