using UserManagementService.Proto.Server;

namespace AccountDeletionScheduler.Configurations;

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
        service.AddGrpcClient<ScheduledDeletionCountService.ScheduledDeletionCountServiceClient>(x =>
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
