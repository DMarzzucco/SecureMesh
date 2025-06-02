using System;

namespace HangfireUserServer.Server.Interfaces;

public interface IUserServices
{
    Task CountedDeletedAsync(int id);
}
