using AutoMapper;
using Microsoft.OpenApi.Models;
using User.Configuration.DbConfiguration;
using User.Mapper;
using User.Utils.Filters;
using System.Text.Json.Serialization;
using User.Module.Service.Interface;
using User.Module.Service;
using User.Module.Repository;
using User.Module.Repository.Interface;
using User.Module.Validations;
using User.Module.Validations.Interface;
using User.Module.Stubs.Maps;
using User.Module.Stubs.Handlers;

namespace User.Extensions;

public static partial class ServiceBuilderExtensions
{
    public static IServiceCollection AddServiceBuilderExtensions(this IServiceCollection services,
        IConfiguration configuration)
    {
        //database connection
        services.AddDatabaseConnection(configuration);
        //gRPC
        services.AddGrpc();
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
        services.AddScoped<IUserValidation, UserValidation>();
        services.AddScoped<IUserService, UserServices>();
        services.AddScoped<IUserRepository, UserRepository>();
       
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