namespace Filedash.StartUp.IoC;

public static class WebServiceExtensions
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
        => services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
}