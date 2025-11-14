using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Infrastructure.Database;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<DirectoryServiceDbContext>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}