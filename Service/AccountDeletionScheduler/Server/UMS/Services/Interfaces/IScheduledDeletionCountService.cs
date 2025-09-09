using System;

namespace AccountDeletionScheduler.Server.UMS.Services.Interfaces;

public interface IScheduledDeletionService
{
    Task CountedDeletion(int userId);
}
