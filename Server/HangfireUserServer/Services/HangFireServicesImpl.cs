using Grpc.Core;
using Hangfire;
using HangfireUserServer.Protos;
using HangfireUserServer.Server.Interfaces;

namespace HangfireUserServer.Services;

public class HangFireServicesImpl(IBackgroundJobClient backgroundJobClient, IAuthServices authServices) : HangFireServicesGrpc.HangFireServicesGrpcBase
{
    private readonly IBackgroundJobClient backgroundJobClient = backgroundJobClient;
    private readonly IAuthServices authServices = authServices;

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
        this.authServices.CountedDeletedAsync(id).GetAwaiter().GetResult();
    } 
}
