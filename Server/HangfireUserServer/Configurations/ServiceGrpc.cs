using System;
using AuthHangFire.Proto;

namespace HangfireUserServer.Configurations;

public static class ServiceGrpc
{
    public static IServiceCollection AddGrpcSerivceClient(this IServiceCollection service)
    {
        service.AddGrpc();
        // just for dev
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        service.AddGrpcClient<AuthHangFireService.AuthHangFireServiceClient>(x =>
        {
            // x.Address = new Uri("https://localhost:5090");

            x.Address = new Uri("https://auth:5090");

            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = httpClientHandler;
            });
        });

        return service;
    }
}
