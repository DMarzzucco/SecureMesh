namespace User.Utils.Exceptions;

/// <summary>
/// 409
/// </summary>
public class ConflictExceptions(string message) : Exception(message) { }
/// <summary>
/// 403
/// </summary>
public class ForbiddenExceptions(string message) : Exception(message) { }

/// <summary>
/// 400
/// </summary>
public class BadRequestExceptions(string message) : Exception(message) { }
/// <summary>
/// 404
/// </summary>
public class NotFoundExceptions(string message) : Exception(message) { }