﻿using AutoMapper;
using Microsoft.OpenApi.Models;
using User.Configuration.DbConfiguration;
using User.Mapper;
using User.Utils.Filters;
using System.Text.Json.Serialization;
using User.Configuration.Swagger.Filter;
using User.Module.Service.Interface;
using User.Module.Service;
using User.Module.Repository;
using User.Module.Repository.Interface;
using User.Module.Validations;
using User.Module.Validations.Interface;
using User.Module.Stubs.Maps;
using User.Module.Stubs.Handlers;
using User.Configuration;
using User.Server.Interfaces;
using User.Server;

namespace User.Extensions;

public static partial class ServiceBuilderExtensions
{
    public static IServiceCollection AddServiceBuilderExtensions(this IServiceCollection services,
        IConfiguration configuration)
    {
        //database connection
        services.AddDatabaseConnection(configuration);
        //gRPC
        services.AddServiceGrpcClient();
        //controller configuration
        services.AddControllers(static op =>
        {
            op.Filters.Add(typeof(GlobalFilterExceptions));
        }).AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.PropertyNamingPolicy = null;
            o.JsonSerializerOptions.WriteIndented = true;
            o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        //service scope
        services.AddScoped<GlobalFilterExceptions>();
        services.AddScoped<MapResponseGrpc>();
        services.AddScoped<HandlerGrpcExceptions>();
        services.AddScoped<IHangFireServices, HangFireService>();
        services.AddScoped<IUserValidation, UserValidation>();
        services.AddScoped<IUserService, UserServices>();
        services.AddScoped<IUserRepository, UserRepository>();
        //swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(e =>
        {
            e.EnableAnnotations();
            e.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "User API",
                Version = "0.1",
                Description = "User API"
            });
            e.SchemaFilter<SwaggerSchemaFilter>();
        });
        //CorsPolicy
        services.AddCors(p =>
        {
            p.AddPolicy("CorsPolicy", e =>
            {
                e.WithOrigins(
                   "https://localhost:5090",
                    //"https://auth:5090",
                   "https://localhost:8888"
                    );
                e.AllowCredentials();
                e.AllowAnyHeader();
            });
        });
        //mapper
        var mappConfig = new MapperConfiguration(conf =>
        {
            conf.AddProfile<MapperProfile>();
        });
        IMapper mapper = mappConfig.CreateMapper();
        services.AddSingleton(mapper);
        services.AddMvc();

        return services;
    }

}