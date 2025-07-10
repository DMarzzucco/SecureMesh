using System;
using Auth.Utils.Exceptions;
using User;

namespace Auth.Server.Helper;

public class HandleGrpcError
{
    public void InvokeError(ErrorResponse error)
    {
        switch (error.StatusCode)
        {
            case 400:
                throw new BadRequestExceptions(error.Message);
            case 403:
                throw new ForbiddenExceptions(error.Message);
            case 404:
                throw new KeyNotFoundException(error.Message);
            case 409:
                throw new ConflictExceptions(error.Message);
            case > 0:
                throw new Exception($"Unhandled gRPC error {error.Message}");

        }
    }
}
