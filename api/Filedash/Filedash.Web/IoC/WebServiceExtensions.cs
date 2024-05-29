using Hangfire;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

namespace Filedash.Web.IoC;

public static class WebServiceExtensions
{
    public static IServiceCollection AddWebServices(
        this IServiceCollection services, 
        IConfiguration configuration,
        ILoggingBuilder loggingBuilder)
    {
        services.AddControllers();

        services.Configure<FormOptions>(options =>
        {
            options.ValueLengthLimit = configuration
                .GetSection(nameof(options.ValueLengthLimit))
                .Get<int>();

            options.MultipartBodyLengthLimit = configuration
                .GetSection(nameof(options.MultipartBodyLengthLimit))
                .Get<int>();
        });

        services.Scan(selector => selector
            .FromCallingAssembly()
            .AddClasses()
            .AsMatchingInterface());
        
        loggingBuilder
            .UseSerilog(configuration);

        return services
            .AddCorsPolicy()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }

    private static ILoggingBuilder UseSerilog(
        this ILoggingBuilder builderLogging, 
        IConfiguration configuration)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builderLogging.ClearProviders();
        builderLogging.AddSerilog(logger);

        return builderLogging;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        => services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition")
                    .WithOrigins("http://localhost:5173");
            });
        });
}