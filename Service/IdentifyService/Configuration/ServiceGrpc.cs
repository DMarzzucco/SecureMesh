using System.Net.Security;
using UserManagementService.Proto.Server;

namespace IdentifyService.Configuration;

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

        var http2Handler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
            }
        };

        /// UMS 
        service.AddGrpcClient<IdpFacedeService.IdpFacedeServiceClient>(x =>
        {
            // x.Address = new Uri("https://localhost:7080");
            x.Address = new Uri("https://ums:7080");

            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = httpClientHandler;
            });
        });

        return service;
    }
}
