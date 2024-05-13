using Microsoft.Extensions.DependencyInjection;

namespace Filedash.Domain.IoC;

public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
        => services.Scan(selector => selector
            .FromCallingAssembly()
            .AddClasses()
            .AsMatchingInterface());
}