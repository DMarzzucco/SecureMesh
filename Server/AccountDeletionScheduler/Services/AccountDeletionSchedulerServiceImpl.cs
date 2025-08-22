using Grpc.Core;
using Hangfire;
using AccountDeletionScheduler.Server.UMS.Services.Interfaces;
using AccountDeletionSchedulerServer.Protos;

namespace AccountDeletionScheduler.Services;

public class AccountDeletionSchedulerServiceImpl(IBackgroundJobClient backgroundJobClient, IScheduledDeletionService scheduledDeletionService) : AccountDeletionSchedulerService.AccountDeletionSchedulerServiceBase
{
    private readonly IBackgroundJobClient backgroundJobClient = backgroundJobClient;
    private readonly IScheduledDeletionService scheduledDeletionService = scheduledDeletionService;

    /// <summary>
    /// Sheduled Delation
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<ScheduleResponse> ScheduleDeletion(ScheduleRequest request, ServerCallContext context)
    {
        var jobId = this.backgroundJobClient.Schedule(() => this.CountedDeletedSyncWrapp(request.AuthId), TimeSpan.FromMinutes(10));

        return Task.FromResult(new ScheduleResponse { JobId = jobId });
    }

    /// <summary>
    /// Delete Schedule Id of Database
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<CancelResponse> CancelScheduledJob(CancelRequest request, ServerCallContext context)
    {
        var success = this.backgroundJobClient.Delete(request.JobId);
        return Task.FromResult(new CancelResponse { Success = success });
    }

    /// <summary>
    /// Counted Deleted async
    /// </summary>
    /// <param name="id"></param>
    public void CountedDeletedSyncWrapp(int id)
    {
        this.scheduledDeletionService.CountedDeletion(id).GetAwaiter().GetResult();
    } 
}
