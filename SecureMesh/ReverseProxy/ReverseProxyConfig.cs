﻿using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy.Transforms;
using SecureMesh.ReverseProxy.Clusters;
using SecureMesh.ReverseProxy.Routes;
using Yarp.ReverseProxy.Swagger.Extensions;
using SecureMesh.Configuration.Swagger;
using SecureMesh.ReverseProxy.Policy;

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
            service.AddRateLimiter(options => { options.ConfigureRateLimiting(); });
            
            return service;
        }
    }
}