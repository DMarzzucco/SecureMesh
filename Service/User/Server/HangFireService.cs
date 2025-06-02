using HangfireUserServer.Protos;
using User.Server.Interfaces;

namespace User.Server;

public class HangFireService(HangFireServicesGrpc.HangFireServicesGrpcClient client) : IHangFireServices
{
    private readonly HangFireServicesGrpc.HangFireServicesGrpcClient client = client;

    /// <summary>
    /// Get Schedule Id Key
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string ScheduleIdKey(int id)
    {
        var request = new ScheduleRequest { UserId = id };
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
