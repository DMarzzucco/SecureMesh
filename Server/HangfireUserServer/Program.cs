using Hangfire;
using HangfireUserServer.Configurations;
using HangfireUserServer.Server;
using HangfireUserServer.Server.Interfaces;
using HangfireUserServer.Services;
using HangfireUserServer.Utils;

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

builder.Services.AddScoped<IUserServices, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
// app.UseHangfireDashboard();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    // just in dev
    Authorization = [new AllowAllAuthorizationFilter()]
});
app.MapGrpcService<HangFireServicesImpl>();

app.Run();
