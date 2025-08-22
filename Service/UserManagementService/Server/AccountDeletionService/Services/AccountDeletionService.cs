using System;
using AccountDeletionSchedulerServer.Protos;
using UserManagementService.Server.Hangfire.Services.Interfaces;

namespace UserManagementService.Server.Hangfire.Services;

public class AccountDeletionService(AccountDeletionSchedulerService.AccountDeletionSchedulerServiceClient client) : IHangFireService
{
    private readonly AccountDeletionSchedulerService.AccountDeletionSchedulerServiceClient client = client;

    /// <summary>
    /// Get Schedule Id Key
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string ScheduleIdKey(int id)
    {
        var request = new ScheduleRequest { AuthId = id };
        var response = this.client.ScheduleDeletion(request);

        return response.JobId;
    }

    /// <summary>
    /// Deleted Scheduled Job
    /// </summary>
    /// <param name="jobId"></param>
    public void DeletedScheduledJob(string jobId)
    {
        var request = new CancelRequest { JobId = jobId };
        this.client.CancelScheduledJob(request);
    }
}
