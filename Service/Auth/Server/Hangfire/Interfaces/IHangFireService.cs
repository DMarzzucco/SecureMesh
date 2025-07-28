using System;

namespace Auth.Server.Hangfire.Interfaces;

public interface IHangFireService
{
    string ScheduleIdKey(int id);
    void DeletedScheduledJob(string jobId);
}
