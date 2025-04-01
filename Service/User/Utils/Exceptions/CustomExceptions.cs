namespace User.Utils.Exceptions;

/// <summary>
/// 409
/// </summary>
public class ConflictExceptions : Exception
{
    public ConflictExceptions(string message) : base(message)
    {
        
    }
}
/// <summary>
/// 400
/// </summary>
public class BadRequestExceptions : Exception
{
    public BadRequestExceptions(string message) : base(message)
    {
        
    }
}
/// <summary>
/// 404
/// </summary>
public class NotFoundExceptions : Exception
{
    public NotFoundExceptions(string message) : base(message)
    {
        
    }
}