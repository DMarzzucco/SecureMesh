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
            var inCont = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            var grpcAddress = inCont ?
                new Uri("https://auth:5090") :
                new Uri("https://localhost:5090");

            x.Address = grpcAddress;
            // x.Address = new Uri("https://localhost:5090");
            
            // x.Address = new Uri("https://auth:5090");

            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = httpClientHandler;
            });
        });

        return service;
    }
}
