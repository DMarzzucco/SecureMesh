using System;
using System.Linq.Expressions;
using AccountDeletionScheduler.Jobs.Interfaces;
using Hangfire;

namespace AccountDeletionScheduler.Jobs;

public class JobsSchedulers(IBackgroundJobClient _backgroundJobClient) : IJobSchedulers
{
    private readonly IBackgroundJobClient backgroundJobClient = _backgroundJobClient;

    public string CreateScheduler(Expression<Action> methodCall, TimeSpan delay)
    {
        var jobId = this.backgroundJobClient.Schedule(methodCall, delay);
        return jobId;
    }

    public bool DeleteScheduler(string jobId)
    {
        this.backgroundJobClient.Delete(jobId);
        return true;
    }
}
