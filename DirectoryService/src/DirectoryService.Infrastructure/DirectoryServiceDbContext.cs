using DirectoryService.Domain.Enities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure;

public class DirectoryServiceDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public DirectoryServiceDbContext()
    {
    }

    public DirectoryServiceDbContext(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Location> Locations { get; set; }

    public DbSet<Position> Positions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("ltree");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DirectoryServiceDb"));
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }
}