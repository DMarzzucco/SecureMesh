using System;

namespace UserManagementService.Server.Hangfire.Services.Interfaces;

public interface IHangFireService
{
    string ScheduleIdKey(int id);
    void DeletedScheduledJob(string jobId);
}
