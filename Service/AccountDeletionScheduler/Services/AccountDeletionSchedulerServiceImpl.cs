using Grpc.Core;
using AccountDeletionScheduler.Server.UMS.Services.Interfaces;
using AccountDeletionSchedulerServer.Protos;
using AccountDeletionScheduler.Jobs.Interfaces;

namespace AccountDeletionScheduler.Services;

public class AccountDeletionSchedulerServiceImpl(IScheduledDeletionService scheduledDeletionService, IJobSchedulers jobSchedulers) : AccountDeletionSchedulerService.AccountDeletionSchedulerServiceBase
{
    private readonly IScheduledDeletionService scheduledDeletionService = scheduledDeletionService;
    private readonly IJobSchedulers _jobSchedulers = jobSchedulers;

    /// <summary>
    /// Sheduled Delation
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<ScheduleResponse> ScheduleDeletion(ScheduleRequest request, ServerCallContext context)
    {
        var jobId = this._jobSchedulers.CreateScheduler(() => this.CountedDeletedSyncWrapp(request.AuthId), TimeSpan.FromMinutes(10));

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
        var success = this._jobSchedulers.DeleteScheduler(request.JobId);
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
