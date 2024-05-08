namespace Filedash.Web.IoC;

public static class WebServiceExtensions
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.Scan(selector => selector
            .FromCallingAssembly()
            .AddClasses()
            .AsMatchingInterface());

        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }
}