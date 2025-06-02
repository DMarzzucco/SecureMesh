using System;

namespace User.Server.Interfaces;

public interface IHangFireServices
{
    string ScheduleIdKey(int id);
    void DeletedScheduledJob(string jobId);
}
