using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy.Transforms;
using SecureMesh.ReverseProxy.Clusters;
using SecureMesh.ReverseProxy.Routes;
using Yarp.ReverseProxy.Swagger.Extensions;
using SecureMesh.Configuration.Swagger;

namespace SecureMesh.ReverseProxy
{
    public static class ReverseProxyConfig
    {
        public static IServiceCollection AddReverseProxyConfig(this IServiceCollection service,
            IConfiguration configuration)
        {
            service.AddReverseProxy().ConfigureHttpClient((context, handler) =>
            //Just for dev 
                {
                    if (handler is SocketsHttpHandler socketsHttpHandler)
                    {
                        socketsHttpHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                        {
                            RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
                        };
                    }
                })
                .LoadFromMemory(
                    RoutesDefinitions.GetRoutes(), ClustersDefinitions.GetCluster()
                ).AddSwagger(SwaggerDocumentProxy.GetSwaggerConfig())
                .AddTransforms(tr =>
                {
                    tr.AddRequestTransform(async ctx =>
                    {
                        var authHeader = ctx.HttpContext.Request.Headers["Authorization"].ToString();

                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                        {
                            var token = authHeader.Substring("Bearer ".Length).Trim();
                            ctx.ProxyRequest.Headers.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        }
                    });
                });
            // Rate Limiter Config
            service.AddRateLimiter(options =>
            {
                // basic rate limiting policy
                options.AddFixedWindowLimiter("rt-sl", op =>
                {
                    op.PermitLimit = 2;
                    op.Window = TimeSpan.FromMinutes(6);
                    //op.QueueLimit = 1;
                    //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // password update policy
                options.AddFixedWindowLimiter("password-update", op =>
                {
                    op.PermitLimit = 1;
                    op.Window = TimeSpan.FromMinutes(15);
                    //op.QueueLimit = 1;
                    //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // update credentials (Username, fullname, email)
                options.AddFixedWindowLimiter("update-credentials", op =>
                {
                    op.PermitLimit = 1;
                    op.Window = TimeSpan.FromDays(60);
                    //op.QueueLimit = 1;
                    //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                
                // Workers (Email Verification & Password recuperation)
                options.AddFixedWindowLimiter("workers-limit", op =>
                {
                    op.PermitLimit = 1;
                    op.Window = TimeSpan.FromMinutes(20);
                    //op.QueueLimit = 1;
                    //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            return service;
        }
    }
}