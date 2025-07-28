using System;

namespace HangfireUserServer.Server.Interfaces;

public interface IAuthServices
{
    Task CountedDeletedAsync(int id);
}
