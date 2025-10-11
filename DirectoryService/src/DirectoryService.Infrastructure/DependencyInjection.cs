using DirectoryService.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<DirectoryServiceDbContext>();
        services.AddScoped<ILocationRepository, LocationRepository>();

        return services;
    }
}