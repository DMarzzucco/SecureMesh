using System;
using System.Linq.Expressions;

namespace AccountDeletionScheduler.Jobs.Interfaces;

public interface IJobSchedulers
{
    string CreateScheduler(Expression<Action> methodCall, TimeSpan delay);
    bool DeleteScheduler(string jobId);
}
