using DirectoryService.Domain.Enitties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static DirectoryService.Domain.Constants.LocationConstants;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id);

        builder.ComplexProperty(l => l.Name, p =>
        {
            p.Property(l => l.Value)
                .HasColumnName("name")
                .HasMaxLength(NAME_MAX_LENGTH)
                .IsRequired();
        });

        builder.ComplexProperty(l => l.Address, p =>
        {
            p.Property(l => l.Value)
                .HasColumnName("address")
                .HasMaxLength(ADDRESS_MAX_LENGTH)
                .IsRequired();
        });

        builder.ComplexProperty(l => l.Timezone, p =>
        {
            p.Property(l => l.Value)
                .HasColumnName("time_zone")
                .HasMaxLength(TIMEZONE_MAX_LENGTH)
                .IsRequired();
        });

        builder.Property(l => l.IsActive)
            .HasColumnName("is_active");

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasMany(d => d.DepartmentLocations)
            .WithOne(d => d.Location)
            .HasForeignKey(d => d.LocationId);
    }
}