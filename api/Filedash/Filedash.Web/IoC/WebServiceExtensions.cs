using Microsoft.AspNetCore.Http.Features;

namespace Filedash.Web.IoC;

public static class WebServiceExtensions
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
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

        return services
            .AddCorsPolicy()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
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
                    .WithOrigins("http://localhost:5173");
            });
        });
}