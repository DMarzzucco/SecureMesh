using System;
using System.Net.Security;
using AccountDeletionSchedulerServer.Protos;
using IdentifyService.Proto.Server;
using SessionsManagement.Protos;
using User;

namespace UserManagementService.Configuration.gRPC;

public static class ServersGrpc
{
        public static IServiceCollection AddGrpcService(this IServiceCollection service)
    {
        service.AddGrpc();
        // just for dev
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var http2Handler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
            }
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
        /// Sessions server
        service.AddGrpcClient<SessionsManagementServiceGrpc.SessionsManagementServiceGrpcClient>(x =>
        {
            // x.Address = new Uri("https://localhost:6070");
            x.Address = new Uri("https://sessions:6070");

            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = http2Handler;
            });
        });
        // IDP server
        service.AddGrpcClient<RemoveIdpRelationService.RemoveIdpRelationServiceClient>(x =>
        {
            // x.Address = new Uri("https://localhost:5090");
            x.Address = new Uri("https://idp:5090");

            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = http2Handler;
            });
        });
        /// hangifre server
        service.AddGrpcClient<AccountDeletionSchedulerService.AccountDeletionSchedulerServiceClient>(x =>
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
