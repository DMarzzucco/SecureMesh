using Hangfire;
using AccountDeletionScheduler.Configurations;
using AccountDeletionScheduler.Server.UMS.Services;
using AccountDeletionScheduler.Server.UMS.Services.Interfaces;
using AccountDeletionScheduler.Services;
using AccountDeletionScheduler.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.WebHost.ConfigureKestrel(op =>
{
    op.ListenAnyIP(3434, listen =>
    {
        listen.UseHttps();
        listen.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});
// Add services to the container.
builder.Services.AddGrpcSerivceClient();
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddGrpc();

builder.Services.AddScoped<IScheduledDeletionService, ScheduledDeletionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
// app.UseHangfireDashboard();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    // just in dev
    Authorization = [new AllowAllAuthorizationFilter()]
});
app.MapGrpcService<AccountDeletionSchedulerServiceImpl>();

app.Run();
