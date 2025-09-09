using AccountDelationSchedulerTest.Helper;
using AccountDeletionScheduler.Jobs.Interfaces;
using AccountDeletionScheduler.Server.UMS.Services.Interfaces;
using AccountDeletionScheduler.Services;
using AccountDeletionSchedulerServer.Protos;
using Grpc.Core;
using Moq;

namespace AccountDelationSchedulerTest;

public class AccountDelationSchedulerServiceTesting
{
    private readonly Mock<IScheduledDeletionService> _scheduledDeletionMock;
    private readonly Mock<IJobSchedulers> _jobScheduler;
    private readonly AccountDeletionSchedulerServiceImpl _service;
    private readonly ServerCallContext _context;

    public AccountDelationSchedulerServiceTesting()
    {
        this._scheduledDeletionMock = new Mock<IScheduledDeletionService>();
        this._jobScheduler = new Mock<IJobSchedulers>();

        this._service = new AccountDeletionSchedulerServiceImpl(
            this._scheduledDeletionMock.Object,
            this._jobScheduler.Object
        );

        this._context = TestServerCallContext.Create();
    }

    [Fact]
    public async Task ScheduleDeletion_Should_Return_JobId()
    {
        var request = new ScheduleRequest { AuthId = 42 };

        this._jobScheduler.Setup(j =>
            j.CreateScheduler(
                It.IsAny<System.Linq.Expressions.Expression<Action>>(), It.IsAny<TimeSpan>()))
                .Returns("123");

        var result = await this._service.ScheduleDeletion(request, _context);

        Assert.Equal("123", result.JobId);
        this._jobScheduler
            .Verify(j => j
                .CreateScheduler(It.IsAny<System.Linq.Expressions.Expression<Action>>(), TimeSpan.FromMinutes(10)), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    public async Task CancelScheduledJob_Should_Return_Success_Based_On_Delete(bool expected )
    {
        var request = new CancelRequest { JobId = "123" };

        this._jobScheduler
            .Setup(j => j.DeleteScheduler("123"))
            .Returns(true);

        var result = await _service.CancelScheduledJob(request, _context);

        Assert.Equal(expected, result.Success);

        this._jobScheduler.Verify(j => j.DeleteScheduler("123"), Times.Once);
    }

    [Fact]
    public void CountedDeletedSyncWrapp_Should_Call_Service()
    {
        this._scheduledDeletionMock
            .Setup(s => s.CountedDeletion(99))
                              .Returns(Task.CompletedTask);

        this._service.CountedDeletedSyncWrapp(99);

        this._scheduledDeletionMock.Verify(s => s.CountedDeletion(99), Times.Once);
    }
}
