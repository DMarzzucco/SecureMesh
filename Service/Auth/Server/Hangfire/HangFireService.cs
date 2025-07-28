using System;
using Auth.Server.Hangfire.Interfaces;
using HangfireUserServer.Protos;

namespace Auth.Server.Hangfire;

public class HangFireService(HangFireServicesGrpc.HangFireServicesGrpcClient client) : IHangFireService
{
    private readonly HangFireServicesGrpc.HangFireServicesGrpcClient client = client;
    
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
