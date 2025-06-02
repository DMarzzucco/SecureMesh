using System;
using HangfireUserServer.Protos;

namespace User.Configuration;

public static class ServiceGrpcClient
{
    public static IServiceCollection AddServiceGrpcClient(this IServiceCollection service)
    {
        service.AddGrpc();
        // just for dev
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        service.AddGrpcClient<HangFireServicesGrpc.HangFireServicesGrpcClient>(x =>
        {
            // x.Address = new Uri("https://localhost:3434");
            x.Address = new Uri("https://hangfire:3434");
            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = httpClientHandler;
            });
        });

        return service;
    }
}
