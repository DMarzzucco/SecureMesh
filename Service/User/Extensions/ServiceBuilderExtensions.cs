using AutoMapper;
using User.Configuration.DbConfiguration;
using User.Mapper;
using User.Utils.Filters;

namespace User.Extensions;

public static partial class ServiceBuilderExtensions
{
    public static IServiceCollection AddServiceBuilderExtensions(this IServiceCollection services,
        IConfiguration configuration)
    {
        //database connection
        services.AddDatabaseConnection(configuration);
        //controller configuration
        services.AddControllers(static op =>
        {
            op.Filters.Add(typeof(GlobalFilterExceptions));
        });
        //service scope
        services.AddScoped<GlobalFilterExceptions>();
        //swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        //mapper
        var mappConfig = new MapperConfiguration(conf =>
        {
            conf.AddProfile<MapperProfile>();
        });
        IMapper mapper = mappConfig.CreateMapper();
        services.AddSingleton(mapper);
        //
        return services;
    }
    
}