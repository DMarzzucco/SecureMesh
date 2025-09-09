using System;
using AccountDeletionScheduler.Server.UMS.Services.Interfaces;
using UserManagementService.Proto.Server;

namespace AccountDeletionScheduler.Server.UMS.Services;

public class ScheduledDeletionService(ScheduledDeletionCountService.ScheduledDeletionCountServiceClient client) : IScheduledDeletionService
{
    private readonly ScheduledDeletionCountService.ScheduledDeletionCountServiceClient client = client;

    public async Task CountedDeletion(int userId)
    {
        var request = new UserIdToDeleteRequest { UserId = userId };
        try
        {
            await this.client.InvokeCountedAsync(request);
        }
        catch (Exception ex) { throw new Exception($"{ex.Message}"); }
    }
}
