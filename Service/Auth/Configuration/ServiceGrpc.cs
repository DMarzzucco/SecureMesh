using HangfireUserServer.Protos;
using Security.Protos;
using User;

namespace Auth.Configuration;

public static class ServiceGrpc
{
    public static IServiceCollection AddGrpcService(this IServiceCollection service)
    {
        service.AddGrpc();
        // just for dev
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        /// user server
        service.AddGrpcClient<UserServiceGrpc.UserServiceGrpcClient>(x =>
        {
            // x.Address = new Uri("https://localhost:4080");
            x.Address = new Uri("https://user:4080");

            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = httpClientHandler;
            });
        });
        /// Security server
        service.AddGrpcClient<SecurityServiceGrpc.SecurityServiceGrpcClient>(x =>
        {
            // x.Address = new Uri("https://localhost:6070");
            x.Address = new Uri("https://security:6070");

            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = httpClientHandler;
            });
        });
        /// hangifre server
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
