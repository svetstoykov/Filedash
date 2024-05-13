using Filedash.Domain.Interfaces;
using Filedash.Infrastructure.DbContext;
using Filedash.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Filedash.Infrastructure.IoC;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
        => services
            .AddDbContext(configuration)
            .AddServicesFromCallingAssembly()
            .AddFileSettingsConfig(configuration);

    private static IServiceCollection AddFileSettingsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileSettings>(
            configuration.GetSection(nameof(FileSettings)));
        
        return services.AddScoped<IFileSettings>(provider =>
        {
            var options = provider.GetRequiredService<IOptionsMonitor<FileSettings>>();

            return options.CurrentValue;
        });
    }
    
    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration) 
        => services
            .AddDbContext<FiledashDbContext>(cfg => cfg
                .UseSqlServer(configuration.GetConnectionString("FiledashConnectionString")));

    private static IServiceCollection AddServicesFromCallingAssembly(this IServiceCollection services)
        => services.Scan(selector => selector
            .FromCallingAssembly()
            .AddClasses()
            .AsMatchingInterface());
}