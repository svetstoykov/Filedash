using Filedash.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Filedash.Infrastructure.IoC;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
        => services
            .AddDbContext(configuration)
            .AddServicesFromCallingAssembly();
    
    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration) 
        => services
            .AddDbContext<FiledashDbContext>(cfg =>
            cfg.UseSqlServer(configuration.GetConnectionString("FiledashConnectionString")));

    private static IServiceCollection AddServicesFromCallingAssembly(this IServiceCollection services)
        => services.Scan(selector => selector
            .FromCallingAssembly()
            .AddClasses()
            .AsMatchingInterface());
}