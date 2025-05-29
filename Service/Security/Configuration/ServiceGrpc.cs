using User;

namespace Security.Configuration;

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
        service.AddGrpcClient<UserServiceGrpc.UserServiceGrpcClient>(x =>
        {
            //x.Address = new Uri("https://172.31.64.1:4080");
            // x.Address = new Uri("https://localhost:4080");
            x.Address = new Uri("https://user:4080");
            x.ChannelOptionsActions.Add(op =>
            {
                op.HttpHandler = httpClientHandler;
            });
        });

        return service;
    }
}
